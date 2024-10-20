#pragma warning disable 1591

namespace cog1.Literals
{
    public class CommonLiteralsContainer : BaseLiteralsContainer
    {
        public CommonLiteralsContainer() : base() { }
        public CommonLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string Login { get => new CommonLiterals.Login().ExtractLiteral(LocaleCode); set { } }

        public virtual string Logout { get => new CommonLiterals.Logout().ExtractLiteral(LocaleCode); set { } }

        public virtual string Email { get => new CommonLiterals.Email().ExtractLiteral(LocaleCode); set { } }

        public virtual string Name { get => new CommonLiterals.Name().ExtractLiteral(LocaleCode); set { } }

        public virtual string Password { get => new CommonLiterals.Password().ExtractLiteral(LocaleCode); set { } }

        public virtual string ChangePassword { get => new CommonLiterals.ChangePassword().ExtractLiteral(LocaleCode); set { } }

        public virtual string CurrentPassword { get => new CommonLiterals.CurrentPassword().ExtractLiteral(LocaleCode); set { } }

        public virtual string NewPassword { get => new CommonLiterals.NewPassword().ExtractLiteral(LocaleCode); set { } }

        public virtual string ReEnterPassword { get => new CommonLiterals.ReEnterPassword().ExtractLiteral(LocaleCode); set { } }

        public virtual string RememberMe { get => new CommonLiterals.RememberMe().ExtractLiteral(LocaleCode); set { } }

        public virtual string ILostMyPassword { get => new CommonLiterals.ILostMyPassword().ExtractLiteral(LocaleCode); set { } }

        public virtual string Next { get => new CommonLiterals.Next().ExtractLiteral(LocaleCode); set { } }

        public virtual string Previous { get => new CommonLiterals.Previous().ExtractLiteral(LocaleCode); set { } }

        public virtual string ReportExecute { get => new CommonLiterals.ReportExecute().ExtractLiteral(LocaleCode); set { } }

        public virtual string Filter { get => new CommonLiterals.Filter().ExtractLiteral(LocaleCode); set { } }

        public virtual string Commands { get => new CommonLiterals.Commands().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonOk { get => new CommonLiterals.ButtonOk().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonConfirm { get => new CommonLiterals.ButtonConfirm().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonCancel { get => new CommonLiterals.ButtonCancel().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonClose { get => new CommonLiterals.ButtonClose().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonYes { get => new CommonLiterals.ButtonYes().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonNo { get => new CommonLiterals.ButtonNo().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonRetry { get => new CommonLiterals.ButtonRetry().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonUpdate { get => new CommonLiterals.ButtonUpdate().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonSearch { get => new CommonLiterals.ButtonSearch().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonClean { get => new CommonLiterals.ButtonClean().ExtractLiteral(LocaleCode); set { } }

        public virtual string ButtonSend { get => new CommonLiterals.ButtonSend().ExtractLiteral(LocaleCode); set { } }

        public virtual string SureYouWantToContinue { get => new CommonLiterals.SureYouWantToContinue().ExtractLiteral(LocaleCode); set { } }

        public virtual string SureYouWantToExit { get => new CommonLiterals.SureYouWantToExit().ExtractLiteral(LocaleCode); set { } }

        public virtual string ChangesSavedSuccessfully { get => new CommonLiterals.ChangesSavedSuccessfully().ExtractLiteral(LocaleCode); set { } }

        public virtual string Description { get => new CommonLiterals.Description().ExtractLiteral(LocaleCode); set { } }

        public virtual string EndpointType { get => new CommonLiterals.EndpointType().ExtractLiteral(LocaleCode); set { } }

        public virtual string EndpointTypes { get => new CommonLiterals.EndpointTypes().ExtractLiteral(LocaleCode); set { } }

        public virtual string Model { get => new CommonLiterals.Model().ExtractLiteral(LocaleCode); set { } }

        public virtual string SerialNumber { get => new CommonLiterals.SerialNumber().ExtractLiteral(LocaleCode); set { } }

        public virtual string FirmwareVersion { get => new CommonLiterals.FirmwareVersion().ExtractLiteral(LocaleCode); set { } }

        public virtual string Application { get => new CommonLiterals.Application().ExtractLiteral(LocaleCode); set { } }

        public virtual string Applications { get => new CommonLiterals.Applications().ExtractLiteral(LocaleCode); set { } }

        public virtual string Application_CoreManager { get => new CommonLiterals.Application_CoreManager().ExtractLiteral(LocaleCode); set { } }

        public virtual string Year { get => new CommonLiterals.Year().ExtractLiteral(LocaleCode); set { } }

        public virtual string Month { get => new CommonLiterals.Month().ExtractLiteral(LocaleCode); set { } }

        public virtual string FirstName { get => new CommonLiterals.FirstName().ExtractLiteral(LocaleCode); set { } }

        public virtual string LastName { get => new CommonLiterals.LastName().ExtractLiteral(LocaleCode); set { } }

        public virtual string Language { get => new CommonLiterals.Language().ExtractLiteral(LocaleCode); set { } }

        public virtual string UploadImage { get => new CommonLiterals.UploadImage().ExtractLiteral(LocaleCode); set { } }

        public virtual string IconUrl { get => new CommonLiterals.IconUrl().ExtractLiteral(LocaleCode); set { } }

        public virtual string Profile { get => new CommonLiterals.Profile().ExtractLiteral(LocaleCode); set { } }

        public virtual string NewLoginNeeded { get => new CommonLiterals.NewLoginNeeded().ExtractLiteral(LocaleCode); set { } }

        public virtual string PleaseEnterFirstName { get => new CommonLiterals.PleaseEnterFirstName().ExtractLiteral(LocaleCode); set { } }

        public virtual string PleaseEnterLastName { get => new CommonLiterals.PleaseEnterLastName().ExtractLiteral(LocaleCode); set { } }

        public virtual string PleaseEnterEmail { get => new CommonLiterals.PleaseEnterEmail().ExtractLiteral(LocaleCode); set { } }

        public virtual string PleaseEnterPassword { get => new CommonLiterals.PleaseEnterPassword().ExtractLiteral(LocaleCode); set { } }

        public virtual string PleaseEnterDescription { get => new CommonLiterals.PleaseEnterDescription().ExtractLiteral(LocaleCode); set { } }

        public virtual string PleaseEnterValue { get => new CommonLiterals.PleaseEnterValue().ExtractLiteral(LocaleCode); set { } }

        public virtual string PleaseEnterCoordinates { get => new CommonLiterals.PleaseEnterCoordinates().ExtractLiteral(LocaleCode); set { } }

        public virtual string PleaseCompleteMandatoryFields { get => new CommonLiterals.PleaseCompleteMandatoryFields().ExtractLiteral(LocaleCode); set { } }

        public virtual string PasswordMismatch { get => new CommonLiterals.PasswordMismatch().ExtractLiteral(LocaleCode); set { } }

        public virtual string GeneralPermissions { get => new CommonLiterals.GeneralPermissions().ExtractLiteral(LocaleCode); set { } }

        public virtual string Reports { get => new CommonLiterals.Reports().ExtractLiteral(LocaleCode); set { } }

        public virtual string Add { get => new CommonLiterals.Add().ExtractLiteral(LocaleCode); set { } }

        public virtual string Edit { get => new CommonLiterals.Edit().ExtractLiteral(LocaleCode); set { } }

        public virtual string Delete { get => new CommonLiterals.Delete().ExtractLiteral(LocaleCode); set { } }

        public virtual string WordYes { get => new CommonLiterals.WordYes().ExtractLiteral(LocaleCode); set { } }

        public virtual string WordNo { get => new CommonLiterals.WordNo().ExtractLiteral(LocaleCode); set { } }

        public virtual string Latitude { get => new CommonLiterals.Latitude().ExtractLiteral(LocaleCode); set { } }

        public virtual string Longitude { get => new CommonLiterals.Longitude().ExtractLiteral(LocaleCode); set { } }

        public virtual string Notification { get => new CommonLiterals.Notification().ExtractLiteral(LocaleCode); set { } }

        public virtual string Notifications { get => new CommonLiterals.Notifications().ExtractLiteral(LocaleCode); set { } }

        public virtual string Day { get => new CommonLiterals.Day().ExtractLiteral(LocaleCode); set { } }

        public virtual string Days { get => new CommonLiterals.Days().ExtractLiteral(LocaleCode); set { } }

        public virtual string Time { get => new CommonLiterals.Time().ExtractLiteral(LocaleCode); set { } }

        public virtual string Parameters { get => new CommonLiterals.Parameters().ExtractLiteral(LocaleCode); set { } }

        public virtual string Enabled_Female { get => new CommonLiterals.Enabled_Female().ExtractLiteral(LocaleCode); set { } }

        public virtual string Enabled_Male { get => new CommonLiterals.Enabled_Male().ExtractLiteral(LocaleCode); set { } }

        public virtual string Suspend { get => new CommonLiterals.Suspend().ExtractLiteral(LocaleCode); set { } }

        public virtual string Unsuspend { get => new CommonLiterals.Unsuspend().ExtractLiteral(LocaleCode); set { } }

        public virtual string Username { get => new CommonLiterals.Username().ExtractLiteral(LocaleCode); set { } }

        public virtual string MultiSelect_All_Male { get => new CommonLiterals.MultiSelect_All_Male().ExtractLiteral(LocaleCode); set { } }

        public virtual string MultiSelect_All_Female { get => new CommonLiterals.MultiSelect_All_Female().ExtractLiteral(LocaleCode); set { } }

        public virtual string MultiSelect_X_Selected_Male { get => new CommonLiterals.MultiSelect_X_Selected_Male().ExtractLiteral(LocaleCode); set { } }

        public virtual string MultiSelect_X_Selected_Female { get => new CommonLiterals.MultiSelect_X_Selected_Female().ExtractLiteral(LocaleCode); set { } }

        public virtual string MultiSelect_X_No_Selected_Male { get => new CommonLiterals.MultiSelect_X_No_Selected_Male().ExtractLiteral(LocaleCode); set { } }

        public virtual string MultiSelect_X_No_Selected_Female { get => new CommonLiterals.MultiSelect_X_No_Selected_Female().ExtractLiteral(LocaleCode); set { } }

        public virtual string Legend { get => new CommonLiterals.Legend().ExtractLiteral(LocaleCode); set { } }

        public virtual string MultiSelect_Find { get => new CommonLiterals.MultiSelect_Find().ExtractLiteral(LocaleCode); set { } }

        public virtual string None_Female { get => new CommonLiterals.None_Female().ExtractLiteral(LocaleCode); set { } }

        public virtual string Select { get => new CommonLiterals.Select().ExtractLiteral(LocaleCode); set { } }

        public virtual string CurrentInformation { get => new CommonLiterals.CurrentInformation().ExtractLiteral(LocaleCode); set { } }

        public virtual string Dashboard { get => new CommonLiterals.Dashboard().ExtractLiteral(LocaleCode); set { } }

        public virtual string Dashboards { get => new CommonLiterals.Dashboards().ExtractLiteral(LocaleCode); set { } }

        public virtual string Descargas { get => new CommonLiterals.Descargas().ExtractLiteral(LocaleCode); set { } }

        public virtual string TotalRowCount { get => new CommonLiterals.TotalRowCount().ExtractLiteral(LocaleCode); set { } }

        public virtual string Details { get => new CommonLiterals.Details().ExtractLiteral(LocaleCode); set { } }

        public virtual string Category { get => new CommonLiterals.Category().ExtractLiteral(LocaleCode); set { } }

        public virtual string Categories { get => new CommonLiterals.Categories().ExtractLiteral(LocaleCode); set { } }

        public string[] WeekdayNames
        {
            get => new string[]
            {
                WeekdayNames_Sunday,
                WeekdayNames_Monday,
                WeekdayNames_Tuesday,
                WeekdayNames_Wednesday,
                WeekdayNames_Thursday,
                WeekdayNames_Friday,
                WeekdayNames_Saturday,
            };
            set { }
        }

        public virtual string WeekdayNames_Sunday { get => new CommonLiterals.WeekdayNames_Sunday().ExtractLiteral(LocaleCode); set { } }

        public virtual string WeekdayNames_Monday { get => new CommonLiterals.WeekdayNames_Monday().ExtractLiteral(LocaleCode); set { } }

        public virtual string WeekdayNames_Tuesday { get => new CommonLiterals.WeekdayNames_Tuesday().ExtractLiteral(LocaleCode); set { } }

        public virtual string WeekdayNames_Wednesday { get => new CommonLiterals.WeekdayNames_Wednesday().ExtractLiteral(LocaleCode); set { } }

        public virtual string WeekdayNames_Thursday { get => new CommonLiterals.WeekdayNames_Thursday().ExtractLiteral(LocaleCode); set { } }

        public virtual string WeekdayNames_Friday { get => new CommonLiterals.WeekdayNames_Friday().ExtractLiteral(LocaleCode); set { } }

        public virtual string WeekdayNames_Saturday { get => new CommonLiterals.WeekdayNames_Saturday().ExtractLiteral(LocaleCode); set { } }

        public string[] WeekdayNames_Short
        {
            get => new string[]
            {
                new CommonLiterals.WeekdayNames_Short_Sunday().ExtractLiteral(LocaleCode),
                new CommonLiterals.WeekdayNames_Short_Monday().ExtractLiteral(LocaleCode),
                new CommonLiterals.WeekdayNames_Short_Tuesday().ExtractLiteral(LocaleCode),
                new CommonLiterals.WeekdayNames_Short_Wednesday().ExtractLiteral(LocaleCode),
                new CommonLiterals.WeekdayNames_Short_Thursday().ExtractLiteral(LocaleCode),
                new CommonLiterals.WeekdayNames_Short_Friday().ExtractLiteral(LocaleCode),
                new CommonLiterals.WeekdayNames_Short_Saturday().ExtractLiteral(LocaleCode),
            };
            set { }
        }

        public string[] MonthNames
        {
            get => new string[]
            {
                new CommonLiterals.MonthNames_January().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_February().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_March().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_April().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_May().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_June().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_July().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_August().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_September().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_October().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_November().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_December().ExtractLiteral(LocaleCode),
            };
            set { }
        }

        public string[] MonthNames_Short
        {
            get => new string[]
            {
                new CommonLiterals.MonthNames_Short_January().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_February().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_March().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_April().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_May().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_June().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_July().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_August().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_September().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_October().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_November().ExtractLiteral(LocaleCode),
                new CommonLiterals.MonthNames_Short_December().ExtractLiteral(LocaleCode),
            };
            set { }
        }

        public virtual string GeoCodingAdminArea2 { get => new CommonLiterals.GeoCodingAdminArea2().ExtractLiteral(LocaleCode); set { } }

        public virtual string Date { get => new CommonLiterals.Date().ExtractLiteral(LocaleCode); set { } }

        public virtual string SelectFile { get => new CommonLiterals.SelectFile().ExtractLiteral(LocaleCode); set { } }

        public virtual string CommandExecutedSuccessfully { get => new CommonLiterals.CommandExecutedSuccessfully().ExtractLiteral(LocaleCode); set { } }

        public virtual string NoData { get => new CommonLiterals.NoData().ExtractLiteral(LocaleCode); set { } }

        public virtual string TurnedOn { get => new CommonLiterals.TurnedOn().ExtractLiteral(LocaleCode); set { } }

        public virtual string TurnedOff { get => new CommonLiterals.TurnedOff().ExtractLiteral(LocaleCode); set { } }

        public virtual string TurnOn { get => new CommonLiterals.TurnOn().ExtractLiteral(LocaleCode); set { } }

        public virtual string TurnOff { get => new CommonLiterals.TurnOff().ExtractLiteral(LocaleCode); set { } }

        public virtual string Stop { get => new CommonLiterals.Stop().ExtractLiteral(LocaleCode); set { } }

        public virtual string MoveUp { get => new CommonLiterals.MoveUp().ExtractLiteral(LocaleCode); set { } }

        public virtual string MoveDown { get => new CommonLiterals.MoveDown().ExtractLiteral(LocaleCode); set { } }

        public virtual string Dim { get => new CommonLiterals.Dim().ExtractLiteral(LocaleCode); set { } }

        public virtual string Today { get => new CommonLiterals.Today().ExtractLiteral(LocaleCode); set { } }

        public virtual string Tomorrow { get => new CommonLiterals.Tomorrow().ExtractLiteral(LocaleCode); set { } }

        public virtual string Yesterday { get => new CommonLiterals.Yesterday().ExtractLiteral(LocaleCode); set { } }

        public virtual string Now { get => new CommonLiterals.Now().ExtractLiteral(LocaleCode); set { } }

        public virtual string AreaCode { get => new CommonLiterals.AreaCode().ExtractLiteral(LocaleCode); set { } }

        public virtual string PhoneNumber { get => new CommonLiterals.PhoneNumber().ExtractLiteral(LocaleCode); set { } }

        public virtual string NoResultsFound { get => new CommonLiterals.NoResultsFound().ExtractLiteral(LocaleCode); set { } }

        public virtual string Loading { get => new CommonLiterals.Loading().ExtractLiteral(LocaleCode); set { } }

        public virtual string GoBack { get => new CommonLiterals.GoBack().ExtractLiteral(LocaleCode); set { } }

        public virtual string Active_Female { get => new CommonLiterals.Active_Female().ExtractLiteral(LocaleCode); set { } }

        public virtual string Active_Male { get => new CommonLiterals.Active_Male().ExtractLiteral(LocaleCode); set { } }

        public virtual string Activate { get => new CommonLiterals.Activate().ExtractLiteral(LocaleCode); set { } }

        public virtual string Event { get => new CommonLiterals.Event().ExtractLiteral(LocaleCode); set { } }

        public virtual string Events { get => new CommonLiterals.Events().ExtractLiteral(LocaleCode); set { } }

        public virtual string Refresh { get => new CommonLiterals.Refresh().ExtractLiteral(LocaleCode); set { } }

        public virtual string RefreshAll { get => new CommonLiterals.RefreshAll().ExtractLiteral(LocaleCode); set { } }

        public virtual string LanguageEnglishUS { get => new CommonLiterals.LanguageEnglishUS().ExtractLiteral(LocaleCode); set { } }

        public virtual string DimLevel { get => new CommonLiterals.DimLevel().ExtractLiteral(LocaleCode); set { } }

        public virtual string Device { get => new CommonLiterals.Device().ExtractLiteral(LocaleCode); set { } }

        public virtual string Devices { get => new CommonLiterals.Devices().ExtractLiteral(LocaleCode); set { } }

        public virtual string Apply { get => new CommonLiterals.Apply().ExtractLiteral(LocaleCode); set { } }

        public virtual string Normal { get => new CommonLiterals.Normal().ExtractLiteral(LocaleCode); set { } }

        public virtual string Warning { get => new CommonLiterals.Warning().ExtractLiteral(LocaleCode); set { } }

        public virtual string Error { get => new CommonLiterals.Error().ExtractLiteral(LocaleCode); set { } }

        public virtual string Errors { get => new CommonLiterals.Errors().ExtractLiteral(LocaleCode); set { } }

        public virtual string N_Errors { get => new CommonLiterals.N_Errors().ExtractLiteral(LocaleCode); set { } }

        public virtual string Total { get => new CommonLiterals.Total().ExtractLiteral(LocaleCode); set { } }

        public virtual string GroupName_Administrators { get => new CommonLiterals.GroupName_Administrators().ExtractLiteral(LocaleCode); set { } }

        public virtual string GroupName_Everyone { get => new CommonLiterals.GroupName_Everyone().ExtractLiteral(LocaleCode); set { } }

        public virtual string Alert { get => new CommonLiterals.Alert().ExtractLiteral(LocaleCode); set { } }

        public virtual string Alerts { get => new CommonLiterals.Alerts().ExtractLiteral(LocaleCode); set { } }

        public virtual string Comments { get => new CommonLiterals.Comments().ExtractLiteral(LocaleCode); set { } }

        public virtual string WordToken { get => new CommonLiterals.WordToken().ExtractLiteral(LocaleCode); set { } }

        public virtual string Price { get => new CommonLiterals.Price().ExtractLiteral(LocaleCode); set { } }

        public virtual string Unsubscribe { get => new CommonLiterals.Unsubscribe().ExtractLiteral(LocaleCode); set { } }

        public virtual string Result { get => new CommonLiterals.Result().ExtractLiteral(LocaleCode); set { } }

        public virtual string Results { get => new CommonLiterals.Results().ExtractLiteral(LocaleCode); set { } }

        public virtual string Variable { get => new CommonLiterals.Variable().ExtractLiteral(LocaleCode); set { } }

        public virtual string Variables { get => new CommonLiterals.Variables().ExtractLiteral(LocaleCode); set { } }

        public virtual string Binary { get => new CommonLiterals.Binary().ExtractLiteral(LocaleCode); set { } }

        public virtual string Integer { get => new CommonLiterals.Integer().ExtractLiteral(LocaleCode); set { } }

        public virtual string FLoatingPoint { get => new CommonLiterals.FloatingPoint().ExtractLiteral(LocaleCode); set { } }

    }

}

#pragma warning restore 1591