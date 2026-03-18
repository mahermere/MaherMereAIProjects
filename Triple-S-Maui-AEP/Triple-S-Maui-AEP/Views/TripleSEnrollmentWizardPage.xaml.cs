using Triple_S_Maui_AEP.Models;
using Triple_S_Maui_AEP.Services;
using Triple_S_Maui_AEP.ViewModels;

namespace Triple_S_Maui_AEP.Views
{
    public partial class TripleSEnrollmentWizardPage : ContentPage
    {
        private readonly Dictionary<View, Label> _fieldLabels = new();
        private readonly Dictionary<View, string> _fieldKeys = new();

        private static readonly Dictionary<string, string> SpanishLabels = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Scope of Appointment # *"] = "# de Alcance de Cita *",
            ["Selected Plan * (Óptimo Plus, Platino Plus, etc.)"] = "Plan Seleccionado * (Óptimo Plus, Platino Plus, etc.)",
            ["Group Coverage (if applicable)"] = "Cobertura de Grupo (si aplica)",
            ["Group Plan Type (HMO/PPO)"] = "Tipo de Plan Grupal (HMO/PPO)",
            ["Group Monthly Premium"] = "Prima Mensual Grupal",
            ["SSN (group plans only)"] = "Seguro Social (solo planes grupales)",
            ["First Name *"] = "Nombre *",
            ["Last Name *"] = "Apellidos *",
            ["Middle Initial (optional)"] = "Inicial (opcional)",
            ["Sex * (F/M)"] = "Sexo * (F/M)",
            ["Home Phone *"] = "Teléfono del Hogar *",
            ["Alternate Phone"] = "Teléfono Alterno",
            ["Permanent Address Line 1 *"] = "Dirección Permanente Línea 1 *",
            ["Permanent Address Line 2"] = "Dirección Permanente Línea 2",
            ["City *"] = "Ciudad *",
            ["State *"] = "Estado *",
            ["ZIP *"] = "Código Postal *",
            ["Mailing Address Line 1 *"] = "Dirección Postal Línea 1 *",
            ["Mailing Address Line 2"] = "Dirección Postal Línea 2",
            ["Mailing City *"] = "Ciudad Postal *",
            ["Mailing State *"] = "Estado Postal *",
            ["Mailing ZIP *"] = "Código Postal de Correo *",
            ["Medicare Number *"] = "Número de Medicare *",
            ["Other Rx Coverage? * (Yes/No)"] = "¿Otra cobertura Rx? * (Sí/No)",
            ["Other Coverage Name"] = "Nombre de Otra Cobertura",
            ["Other Coverage Member Number"] = "Número de Miembro de Otra Cobertura",
            ["Other Coverage Group Number"] = "Número de Grupo de Otra Cobertura",
            ["Medicaid Program? (Platino plans) (Yes/No)"] = "¿Programa Medicaid? (planes Platino) (Sí/No)",
            ["Medicaid Number (MPI)"] = "Número de Medicaid (MPI)",
            ["Contigo Plus Chronic Condition"] = "Condición Crónica de Contigo Plus",
            ["ContigoEnMente dementia symptoms? (Yes/No)"] = "¿Síntomas de demencia en ContigoEnMente? (Sí/No)",
            ["Applicant Signature Name *"] = "Nombre de Firma del Solicitante *",
            ["Authorized Rep Name"] = "Nombre del Representante Autorizado",
            ["Authorized Rep Address"] = "Dirección del Representante Autorizado",
            ["Authorized Rep Phone"] = "Teléfono del Representante Autorizado",
            ["Relationship to Enrollee"] = "Relación con el Afiliado",
            ["Preferred Spoken Language"] = "Idioma Hablado Preferido",
            ["Preferred Accessible Format"] = "Formato Accesible Preferido",
            ["Do you work? (Yes/No)"] = "¿Trabaja? (Sí/No)",
            ["Does spouse work? (Yes/No)"] = "¿Trabaja su cónyuge? (Sí/No)",
            ["PCP Name"] = "Nombre del PCP",
            ["PCP Phone"] = "Teléfono del PCP",
            ["Text consent? (Yes/No)"] = "¿Consente mensajes de texto? (Sí/No)",
            ["Text Number"] = "Número para Mensajes",
            ["Email consent? (Yes/No)"] = "¿Consiente correo electrónico? (Sí/No)",
            ["Email Address"] = "Correo Electrónico",
            ["Emergency Contact Name"] = "Nombre de Contacto de Emergencia",
            ["Emergency Contact Phone"] = "Teléfono de Contacto de Emergencia",
            ["Are you the retiree? (Yes/No)"] = "¿Es usted el retirado? (Sí/No)",
            ["Retiree Name (if no)"] = "Nombre del Retirado (si no)",
            ["Covering spouse/dependents? (Yes/No/NA)"] = "¿Cubre cónyuge/dependientes? (Sí/No/NA)",
            ["Spouse Name"] = "Nombre del Cónyuge",
            ["Dependent Names"] = "Nombres de Dependientes",
            ["Resident in Long-Term Care facility? (Yes/No)"] = "¿Residente en institución de cuidado prolongado? (Sí/No)",
            ["LTC Institution Name"] = "Nombre de Institución LTC",
            ["LTC Administrator Name"] = "Nombre del Administrador LTC",
            ["LTC Phone"] = "Teléfono LTC",
            ["Current Health Plan"] = "Plan de Salud Actual",
            ["Payment Option (Coupon Book / EFT / Credit Card / Auto Deduction)"] = "Opción de Pago (Cupón / EFT / Tarjeta / Débito Automático)",
            ["Account Holder Name"] = "Nombre del Titular de la Cuenta",
            ["Routing Number"] = "Número de Ruta",
            ["Account Number"] = "Número de Cuenta",
            ["Account Type (Checking/Savings)"] = "Tipo de Cuenta (Corriente/Ahorros)",
            ["Card Type (Visa/Master Card)"] = "Tipo de Tarjeta (Visa/Master Card)",
            ["Card Holder Name"] = "Nombre del Titular de la Tarjeta",
            ["Card Number"] = "Número de Tarjeta",
            ["Expiration MM/YYYY"] = "Expiración MM/AAAA",
            ["Benefit Source (Social Security/RRB)"] = "Fuente de Beneficio (Seguro Social/RRB)",
            ["Helper Name"] = "Nombre del Ayudante",
            ["Helper Relationship"] = "Relación del Ayudante",
            ["Helper Signature"] = "Firma del Ayudante",
            ["Helper NPN (Agents/Brokers)"] = "NPN del Ayudante (Agentes/Corredores)",
            ["Receipt Date"] = "Fecha de Recibo",
            ["Plan ID"] = "ID del Plan",
            ["Effective Date of Coverage"] = "Fecha Efectiva de Cobertura",
            ["BirthDate"] = "Fecha de Nacimiento",
            ["GroupEffectiveDate"] = "Fecha Efectiva Grupal",
            ["SignatureDate"] = "Fecha de Firma",
            ["RetirementDate"] = "Fecha de Retiro",
            ["Last Names *"] = "Apellidos *",
            ["Name *"] = "Nombre *",
            ["Initial"] = "Inicial",
            ["Telephone 1 *"] = "Teléfono 1 *",
            ["Telephone 2"] = "Teléfono 2",
            ["Benefit Plan *"] = "Plan de Beneficio *",
            ["SHIC # (Medicare Number) *"] = "SHIC # (Número de Medicare) *",
            ["Equipment / Supply / Services selected"] = "Equipo / Suministro / Servicios seleccionados",
            ["Provider / Company"] = "Proveedor / Compañía",
            ["Effectivity (month/year)"] = "Efectividad (mes/año)",
            ["Previous Health Plan"] = "Plan de Salud Anterior",
            ["Information Provided By *"] = "Información Provista Por *",
            ["Plan Representative *"] = "Representante del Plan *",
            ["Region *"] = "Región *",
        };

        public TripleSEnrollmentWizardPage()
        {
            InitializeComponent();
            BindingContext = new TripleSEnrollmentWizardViewModel();

            BuildFieldLabels();
            ApplyLanguageToFieldLabels(LanguageService.Instance.CurrentLanguage);
            LanguageService.Instance.LanguageChanged += ApplyLanguageToFieldLabels;
        }

        private void BuildFieldLabels()
        {
            if (Content is not ScrollView scrollView || scrollView.Content is not Layout rootLayout)
            {
                return;
            }

            BuildFieldLabelsRecursive(rootLayout);
        }

        private void BuildFieldLabelsRecursive(Layout layout)
        {
            for (int i = 0; i < layout.Children.Count; i++)
            {
                var child = layout.Children[i];

                if (child is Layout nested)
                {
                    BuildFieldLabelsRecursive(nested);
                    continue;
                }

                if (child is Entry entry)
                {
                    if (_fieldLabels.ContainsKey(entry))
                    {
                        continue;
                    }

                    var key = entry.Placeholder ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    var label = CreateFieldLabel();
                    layout.Children.Insert(i, label);
                    _fieldLabels[entry] = label;
                    _fieldKeys[entry] = key;
                    i++;
                    continue;
                }

                if (child is Picker picker)
                {
                    if (_fieldLabels.ContainsKey(picker))
                    {
                        continue;
                    }

                    var key = picker.Title;
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        key = "Field";
                    }

                    var label = CreateFieldLabel();
                    layout.Children.Insert(i, label);
                    _fieldLabels[picker] = label;
                    _fieldKeys[picker] = key;
                    i++;
                    continue;
                }

                if (child is DatePicker datePicker)
                {
                    if (_fieldLabels.ContainsKey(datePicker))
                    {
                        continue;
                    }

                    var key = datePicker.AutomationId;
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        key = "Date";
                    }

                    var label = CreateFieldLabel();
                    layout.Children.Insert(i, label);
                    _fieldLabels[datePicker] = label;
                    _fieldKeys[datePicker] = key;
                    i++;
                }
            }
        }

        private static Label CreateFieldLabel()
        {
            return new Label
            {
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2C3E50"),
                Margin = new Thickness(0, 4, 0, 0)
            };
        }

        private void ApplyLanguageToFieldLabels(Language language)
        {
            var isEnglish = language == Language.English;

            foreach (var pair in _fieldLabels)
            {
                var view = pair.Key;
                var label = pair.Value;
                var key = _fieldKeys.TryGetValue(view, out var k) ? k : string.Empty;
                var localized = GetLocalizedLabel(key, isEnglish);

                label.Text = localized;

                if (view is Entry entry)
                {
                    entry.Placeholder = localized;
                }
                else if (view is Picker picker)
                {
                    picker.Title = localized;
                }
            }
        }

        private static string GetLocalizedLabel(string key, bool isEnglish)
        {
            if (isEnglish)
            {
                return key;
            }

            if (SpanishLabels.TryGetValue(key, out var translated))
            {
                return translated;
            }

            return key;
        }
    }
}
