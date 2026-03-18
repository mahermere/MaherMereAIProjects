using LiteDB;
using Triple_S_AEP_MAUI_Forms.Models;

namespace Triple_S_AEP_MAUI_Forms.Services;

public class EnrollmentDatabaseService : IDisposable
{
    private const string DatabaseFileName = "enrollments.db";
    private const string EncryptionPassword = "TripleS_AEP_2024_Secure";
    private readonly string _databasePath;
    private ILiteDatabase? _database;
    private static EnrollmentDatabaseService? _instance;
    private static readonly object _lock = new();

    public static EnrollmentDatabaseService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new EnrollmentDatabaseService();
                }
            }
            return _instance;
        }
    }

    private EnrollmentDatabaseService()
    {
        _databasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
    }

    private ILiteDatabase GetDatabase()
    {
        if (_database == null)
        {
            try
            {
                _database?.Dispose();
            }
            catch { }

            var connString = new ConnectionString()
            {
                Filename = _databasePath,
                Password = EncryptionPassword,
                Upgrade = true
            };

            _database = new LiteDatabase(connString);

            var col = _database.GetCollection<EnrollmentRecord>();
            col.EnsureIndex(x => x.BeneficiaryLastName);
            col.EnsureIndex(x => x.CreatedDate);
            col.EnsureIndex(x => x.SOANumber);
        }

        return _database;
    }

    public void AddOrUpdateRecord(EnrollmentRecord record)
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();

            var normalizedSoa = NormalizeSoaNumber(record.SOANumber);
            var existingBySoa = string.IsNullOrEmpty(normalizedSoa)
                ? null
                : col.FindAll().FirstOrDefault(x => NormalizeSoaNumber(x.SOANumber) == normalizedSoa);

            if (record.Id == default)
            {
                if (existingBySoa != null)
                {
                    MergeRecord(existingBySoa, record);
                    existingBySoa.LastModifiedDate = DateTime.Now;
                    col.Update(existingBySoa);
                    record.Id = existingBySoa.Id;
                    return;
                }

                record.CreatedDate = record.CreatedDate == default ? DateTime.Now : record.CreatedDate;
                record.LastModifiedDate = DateTime.Now;
                col.Insert(record);
                return;
            }

            if (existingBySoa != null && existingBySoa.Id != record.Id)
            {
                MergeRecord(existingBySoa, record);
                existingBySoa.LastModifiedDate = DateTime.Now;
                col.Update(existingBySoa);
                col.Delete(record.Id);
                record.Id = existingBySoa.Id;
                return;
            }

            record.LastModifiedDate = DateTime.Now;
            col.Update(record);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to save enrollment record: {ex.Message}", ex);
        }
    }

    public EnrollmentRecord? GetRecordBySoaNumber(string soaNumber)
    {
        try
        {
            var normalized = NormalizeSoaNumber(soaNumber);
            if (string.IsNullOrEmpty(normalized))
            {
                return null;
            }

            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            return col.FindAll().FirstOrDefault(x => NormalizeSoaNumber(x.SOANumber) == normalized);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve enrollment record by SOA number: {ex.Message}", ex);
        }
    }

    public EnrollmentRecord? GetRecordById(ObjectId id)
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            return col.FindById(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve enrollment record: {ex.Message}", ex);
        }
    }

    public IEnumerable<EnrollmentRecord> GetAllRecords()
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            return col.FindAll().OrderByDescending(x => x.CreatedDate).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve enrollment records: {ex.Message}", ex);
        }
    }

    public IEnumerable<EnrollmentRecord> GetRecordsByDate(DateTime date)
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return col.Find(x => x.CreatedDate >= startOfDay && x.CreatedDate < endOfDay)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve records by date: {ex.Message}", ex);
        }
    }

    public IEnumerable<EnrollmentRecord> SearchByBeneficiary(string firstName, string lastName)
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            return col.Find(x =>
                x.BeneficiaryFirstName.Contains(firstName) &&
                x.BeneficiaryLastName.Contains(lastName))
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to search records: {ex.Message}", ex);
        }
    }

    public IEnumerable<EnrollmentRecord> SearchSoaRecords(string soaNumber)
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            return col.Find(x =>
                !string.IsNullOrEmpty(x.SOANumber) &&
                x.SOANumber.Contains(soaNumber) &&
                !string.IsNullOrEmpty(x.SoaFormPdfPath))
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to search SOA records: {ex.Message}", ex);
        }
    }

    public IEnumerable<EnrollmentRecord> GetUnlinkedSoaRecords()
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            return col.Find(x =>
                !string.IsNullOrEmpty(x.SOANumber) &&
                !string.IsNullOrEmpty(x.SoaFormPdfPath))
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get SOA records: {ex.Message}", ex);
        }
    }

    public void DeleteRecord(ObjectId id)
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            col.Delete(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete enrollment record: {ex.Message}", ex);
        }
    }

    public int GetTotalRecordsCount()
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            return col.Count();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get record count: {ex.Message}", ex);
        }
    }

    public int GetRecordsCountByDate(DateTime date)
    {
        try
        {
            var db = GetDatabase();
            var col = db.GetCollection<EnrollmentRecord>();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return col.Count(x => x.CreatedDate >= startOfDay && x.CreatedDate < endOfDay);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get daily record count: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        try
        {
            _database?.Dispose();
            _database = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error disposing database: {ex.Message}");
        }
    }

    private static string? NormalizeSoaNumber(string? soaNumber)
    {
        var normalized = soaNumber?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized.ToUpperInvariant();
    }

    private static void MergeRecord(EnrollmentRecord target, EnrollmentRecord source)
    {
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryFirstName)) target.BeneficiaryFirstName = source.BeneficiaryFirstName;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryLastName)) target.BeneficiaryLastName = source.BeneficiaryLastName;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryMiddleInitial)) target.BeneficiaryMiddleInitial = source.BeneficiaryMiddleInitial;
        if (source.BeneficiaryDOB.HasValue) target.BeneficiaryDOB = source.BeneficiaryDOB;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryPhone)) target.BeneficiaryPhone = source.BeneficiaryPhone;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryAltPhone)) target.BeneficiaryAltPhone = source.BeneficiaryAltPhone;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryEmail)) target.BeneficiaryEmail = source.BeneficiaryEmail;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryAddress1)) target.BeneficiaryAddress1 = source.BeneficiaryAddress1;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryAddress2)) target.BeneficiaryAddress2 = source.BeneficiaryAddress2;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryCity)) target.BeneficiaryCity = source.BeneficiaryCity;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryState)) target.BeneficiaryState = source.BeneficiaryState;
        if (!string.IsNullOrWhiteSpace(source.BeneficiaryZip)) target.BeneficiaryZip = source.BeneficiaryZip;

        if (!string.IsNullOrWhiteSpace(source.AuthorizedRepFirstName)) target.AuthorizedRepFirstName = source.AuthorizedRepFirstName;
        if (!string.IsNullOrWhiteSpace(source.AuthorizedRepLastName)) target.AuthorizedRepLastName = source.AuthorizedRepLastName;
        if (!string.IsNullOrWhiteSpace(source.AuthorizedRepMiddleInitial)) target.AuthorizedRepMiddleInitial = source.AuthorizedRepMiddleInitial;
        if (!string.IsNullOrWhiteSpace(source.AuthorizedRepRelationship)) target.AuthorizedRepRelationship = source.AuthorizedRepRelationship;

        if (!string.IsNullOrWhiteSpace(source.SOANumber)) target.SOANumber = source.SOANumber;
        if (!string.IsNullOrWhiteSpace(source.CampaignNumber)) target.CampaignNumber = source.CampaignNumber;
        if (!string.IsNullOrWhiteSpace(source.CampaignName)) target.CampaignName = source.CampaignName;
        if (!string.IsNullOrWhiteSpace(source.ProductType)) target.ProductType = source.ProductType;
        if (!string.IsNullOrWhiteSpace(source.InitialContactMethod)) target.InitialContactMethod = source.InitialContactMethod;
        target.BeneficiaryWalkedIn = target.BeneficiaryWalkedIn || source.BeneficiaryWalkedIn;
        target.NewToMedicare = target.NewToMedicare || source.NewToMedicare;
        if (!string.IsNullOrWhiteSpace(source.CurrentPlan)) target.CurrentPlan = source.CurrentPlan;
        if (source.LinkedSoaRecordId != null) target.LinkedSoaRecordId = source.LinkedSoaRecordId;

        if (!string.IsNullOrWhiteSpace(source.EnrollmentFormPdfPath)) target.EnrollmentFormPdfPath = source.EnrollmentFormPdfPath;
        if (!string.IsNullOrWhiteSpace(source.SoaFormPdfPath)) target.SoaFormPdfPath = source.SoaFormPdfPath;
        if (!string.IsNullOrWhiteSpace(source.WorkingAgeSurveyPdfPath)) target.WorkingAgeSurveyPdfPath = source.WorkingAgeSurveyPdfPath;

        if (!string.IsNullOrWhiteSpace(source.EnrollmentFormDmsDocumentId)) target.EnrollmentFormDmsDocumentId = source.EnrollmentFormDmsDocumentId;
        if (!string.IsNullOrWhiteSpace(source.SoaFormDmsDocumentId)) target.SoaFormDmsDocumentId = source.SoaFormDmsDocumentId;
        if (!string.IsNullOrWhiteSpace(source.WorkingAgeSurveyDmsDocumentId)) target.WorkingAgeSurveyDmsDocumentId = source.WorkingAgeSurveyDmsDocumentId;

        if (source.EnrollmentUploadStatus != EnrollmentUploadStatus.Pending) target.EnrollmentUploadStatus = source.EnrollmentUploadStatus;
        if (source.SoaUploadStatus != EnrollmentUploadStatus.Pending) target.SoaUploadStatus = source.SoaUploadStatus;
        if (source.WorkingAgeSurveyUploadStatus != EnrollmentUploadStatus.Pending) target.WorkingAgeSurveyUploadStatus = source.WorkingAgeSurveyUploadStatus;

        if (source.SubmittedDate.HasValue) target.SubmittedDate = source.SubmittedDate;
        target.CreatedDate = target.CreatedDate == default
            ? source.CreatedDate
            : (source.CreatedDate != default ? (target.CreatedDate < source.CreatedDate ? target.CreatedDate : source.CreatedDate) : target.CreatedDate);
        target.IsComplete = target.IsComplete || source.IsComplete;
    }
}
