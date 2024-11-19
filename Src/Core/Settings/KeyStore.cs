namespace HUBT_Social_API.Core.Settings
{
    public static class KeyStore
    {
        // Các key cho thông điệp liên quan đến đăng ký và xác minh 
        public const string InvalidInformation = "InvalidInformation";
        public const string RegistrationFailed = "RegistrationFailed";
        public const string UnableToSendOTP = "UnableToSendOTP";
        public const string RegistrationSuccess = "RegistrationSuccess";
        public const string StepOneVerificationSuccess = "StepOneVerificationSuccess";
        public const string AccountLocked = "AccountLocked";
        public const string LoginNotAllowed = "LoginNotAllowed";
        public const string TwoFactorRequired = "TwoFactorRequired";
        public const string InvalidCredentials = "InvalidCredentials";
        public const string VerificationSuccess = "VerificationSuccess";
        public const string OTPVerificationFailed = "OTPVerificationFailed";
        public const string InvalidLanguage = "InvalidLanguage";
        public const string LanguageChanged = "LanguageChanged";
        public const string LanguageChangeFailed = "LanguageChangeFailed";
        public const string DefaultLoginError ="DefaultLoginError";
        
        // Các key cho thông điệp liên quan đến tin nhắn
        public const string InvalidMessageData = "InvalidMessageData";
        public const string MessageSentSuccessfully = "MessageSentSuccessfully";
        public const string FailedToSendMessage = "FailedToSendMessage";
        public const string ChatRoomIdRequired = "ChatRoomIdRequired";
        public const string NoMessagesFound = "NoMessagesFound";
        public const string MessageDeletedSuccessfully = "MessageDeletedSuccessfully";
        public const string FailedToDeleteMessage = "FailedToDeleteMessage";
        public const string ChatRoomIdAndKeywordRequired = "ChatRoomIdAndKeywordRequired";
        public const string MessagesFoundSuccessfully = "MessagesFoundSuccessfully";
        
        // Các key cho thông điệp liên quan đến file và hình ảnh
        public const string ImageUploadedSuccessfully = "ImageUploadedSuccessfully";
        public const string FailedToUploadImage = "FailedToUploadImage";
        public const string FileUploadedSuccessfully = "FileUploadedSuccessfully";
        public const string FailedToUploadFile = "FailedToUploadFile";
        
        // Các key cho thông điệp liên quan đến nhóm trò chuyện
        public const string GroupNameRequired = "GroupNameRequired";
        public const string GroupCreatedSuccessfully = "GroupCreatedSuccessfully";
        public const string FailedToCreateGroup = "FailedToCreateGroup";
        public const string GroupNameUpdatedSuccessfully = "GroupNameUpdatedSuccessfully";
        public const string FailedToUpdateGroup = "FailedToUpdateGroup";
        public const string GroupDeletedSuccessfully = "GroupDeletedSuccessfully";
        public const string FailedToDeleteGroup = "FailedToDeleteGroup";
        public const string GroupNotFound = "GroupNotFound";
        
        // Các key cho thông điệp liên quan đến người dùng
        public const string UserAlreadyExists = "UserAlreadyExists";
        public const string UnableToStoreInDatabase = "UnableToStoreInDatabase";
        public const string EmailVerificationCodeSubject = "EmailVerificationCodeSubject";
        public const string EmailCannotBeEmpty = "EmailCannotBeEmpty";
        public const string EmailUpdated = "EmailUpdated";
        public const string EmailUpdateError = "EmailUpdateError";
        public const string PasswordCannotBeEmpty = "PasswordCannotBeEmpty";
        public const string PasswordUpdated = "PasswordUpdated";
        public const string PasswordUpdateError = "PasswordUpdateError";
        public const string NameUpdated = "NameUpdated";
        public const string NameUpdateError = "NameUpdateError";
        public const string PhoneNumberUpdated = "PhoneNumberUpdated";
        public const string PhoneNumberUpdateError = "PhoneNumberUpdateError";
        public const string GenderUpdated = "GenderUpdated";
        public const string GenderUpdateError = "GenderUpdateError";
        public const string DateOfBirthUpdated = "DateOfBirthUpdated";
        public const string DateOfBirthUpdateError = "DateOfBirthUpdateError";
        public const string GeneralUpdateSuccess = "GeneralUpdateSuccess";
        public const string GeneralUpdateError = "GeneralUpdateError";
        public const string CurrentPasswordAndUsernameCannotBeEmpty = "CurrentPasswordAndUsernameCannotBeEmpty";
        public const string PasswordCorrect = "PasswordCorrect";
        public const string PasswordIncorrect = "PasswordIncorrect";
        public const string UsernameCannotBeEmpty = "UsernameCannotBeEmpty";

        public const string AvatarUpdated = "AvatarUpdated";

        public const string AvatarUpdateError ="AvatarUpdateError";

        
        // Các key cho thông điệp lỗi và OTP
        public const string InvalidRequestError = "InvalidRequest";
        public const string OtpSent = "OtpSent";
        public const string OtpSendError = "OtpSendError";
        public const string OtpVerificationSuccess = "OtpVerificationSuccess";
        public const string OtpInvalid = "OtpInvalid";
        public const string UserNotFound = "UserNotFound";
        public const string UserInfoUpdatedSuccess = "UserInfoUpdatedSuccess";
        public const string UserInfoUpdateError = "UserInfoUpdateError";
        
        // Các key cho lỗi OTP và password
        public const string PasswordCheckEmptyError = "PasswordCheckEmptyError";
        public const string OtpVerifyEmptyError = "OtpVerifyEmptyError";

        public const string InvalidImageData  = "InvalidImageData";
        public const string InvalidFileData = "InvalidFileData";

        

        public const string TokenValid =  "TokenValid";
        public const string AvatarDefaultFemale1 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811655/qqgtqqr9igqnz6pj8zjn.jpg";
        public const string AvatarDefaultFemale2 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811807/ahd7jbrya441tv0wv4h2.jpg";
        public const string AvatarDefaultFemale3 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811831/khszyns5uzh6gf2zncsd.jpg";
        public const string AvatarDefaultFemale4 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811852/hhn3ljzoky2jcz4djivy.jpg";
        public const string AvatarDefaultFemale5 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811872/xthln1h8axw3nwrjqz0l.jpg";
        public const string AvatarDefaultFemale6 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811890/migfj5wqjmk4v6jeuada.jpg";
        public const string AvatarDefaultFemale7 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811913/bnlco4r2fwcsajwg7e7j.jpg";
        public const string AvatarDefaultFemale8 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811930/b6oi4predbmtbqnf2gi4.jpg";
        public const string AvatarDefaultMale1 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811971/ihaipmgkzjyfliavkq3e.jpg";
        public const string AvatarDefaultMale2 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731811992/lbkn7ehp5aqdpkjkqzgf.jpg";
        public const string AvatarDefaultMale3 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731812010/fjt3fwuqk53lnb910qvu.jpg";
        public const string AvatarDefaultMale4 = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1731812037/txbbmvgtpyhnleij7g1s.jpg";
        public const string DefaultUserImage = "https://res.cloudinary.com/dnx8aew1t/image/upload/v1730901747/v5elptamoonvux5xth0a.jpg";

        public static string GetRandomAvatarDefault(Gender gender)
        {
            var random = new Random();
            List<string> avatarUrls;

            // Chọn danh sách avatar dựa trên giới tính
            switch (gender)
            {
                case Gender.Male:
                    avatarUrls = new List<string>
                    {
                        AvatarDefaultMale1,
                        AvatarDefaultMale2,
                        AvatarDefaultMale3,
                        AvatarDefaultMale4
                    };
                    break;

                case Gender.Female:
                    avatarUrls = new List<string>
                    {
                        AvatarDefaultFemale1,
                        AvatarDefaultFemale2,
                        AvatarDefaultFemale3,
                        AvatarDefaultFemale4,
                        AvatarDefaultFemale5,
                        AvatarDefaultFemale6,
                        AvatarDefaultFemale7,
                        AvatarDefaultFemale8
                    };
                    break;

                default:
                    // Trường hợp không xác định giới tính
                    avatarUrls = new List<string> { DefaultUserImage };
                    break;
            }

            // Chọn ngẫu nhiên từ danh sách
            return avatarUrls[random.Next(avatarUrls.Count)];
        }
    }
}
