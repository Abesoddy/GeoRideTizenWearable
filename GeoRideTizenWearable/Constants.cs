namespace GeoRideTizenWearable
{
    public class Constants
    {
        // Tag for log debug
        public const string logTag = "GeoRideTizenWearable";

        // Internet privilege
        public const string internetPrivilege = "http://tizen.org/privilege/internet";

        public const string baseUrlApiGeoride = "https://api.georide.fr/";
        public const string loginEndpointApiGeoride = "user/login";
        public const string trackersEndpointApiGeoride = "user/trackers";
        public const string regenerateTokenEndpointApiGeoride = "user/new-token";

        public const string permissionNotGranted = "L'application ne posséde pas la permission '" + internetPrivilege + "'.";
        public const string noNetwork = "Aucune connexion internet, veuillez vérifier vos réglages.";

        // Errors login
        public const string invalidLogin = "Identifiant incorrects.";
        public const string invalidRequest = "Une erreur est survenue, veuillez réessayer.";

        // Regenerate token
        public const string errorRegenerateToken = "Une erreur est survenue lors du renouvellement de votre token, veuillez réessayer.";
        public const string regenerateToken = "Votre token vient d'être renouvellé avec succès.";

        public const string unlockButtonText = "Déverrouiller";
        public const string lockButtonText = "Verrouiller";
        public const string lockerUnlock = "locker_unlock.png";
        public const string lockerLock = "locker_lock.png";
        public const string lockerEmpty = "locker_empty.png";

        // Form validation
        public const string emailEmpty = "Votre adresse mail doit être renseignée.";
        public const string emailInvalid = "Votre adresse mail n'est pas valide.";
        public const string passwordEmpty = "Votre mot de passe doit être renseignée.";

        // Popup
        public const string iconButtonDone = "done.png";
        public const string iconButtonCancel = "cancel.png";
        public const string textPopupLogout = "Voulez-vous vraiment vous déconnecter ?";
    }
}