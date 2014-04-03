using System;
using System.Windows.Data;
using System.Globalization;

/*
 * CLASSE APENAS PARA TESTE 
 */

namespace ChallengeApp.Utils
{
    public class UserToAvatarConverter : IValueConverter
    {
        public const string PLACEHOLDER = "/Assets/Avatar/placeholder.jpg";
        public const string BRUNOLEMOS  = "http://graph.facebook.com/brunolemos/picture?width=100&height=100";
        public const string GADU        = "http://graph.facebook.com/rodrigovenan/picture?width=100&height=100";
        public const string MAURICIO    = "http://graph.facebook.com/mauriciogior/picture?width=100&height=100";
        public const string BATATA      = "/Assets/Avatar/batata.jpg";
        public const string PSY         = "http://graph.facebook.com/will.pre.psy/picture?width=100&height=100";
        public const string SERGIO      = "http://graph.facebook.com/targaryen.fire/picture?width=100&height=100";
        public const string JULIANO     = "http://graph.facebook.com/juliano.battisti/picture?width=100&height=100";
        public const string BKROST      = "http://graph.facebook.com/bruno.krost/picture?width=100&height=100";
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return PLACEHOLDER;
            var email = value.ToString();

            switch (email)
            {
                case "bruno2":                          return BRUNOLEMOS;
                case "rod":                             return GADU;
                case "mauricio":                        return MAURICIO;
                case "mauriciogior":                    return MAURICIO;
                case "mcgiordalp@gmail.com":            return MAURICIO;
                case "psy":                             return PSY;
                case "sergio":                          return SERGIO;
                case "seergioandrade":                  return SERGIO;
                case "batata":                          return BATATA;
                case "juliano":                         return JULIANO;
            }

            if (email.IndexOf("gadu") >= 0) return GADU;
            else if (email.IndexOf("bruno") >= 0 && email.IndexOf("lemos") >= 0) return BRUNOLEMOS;
            else if (email.IndexOf("mauricio") >= 0 && email.IndexOf("giordano") >= 0) return MAURICIO;
            else if (email.IndexOf("rodrigo") >= 0 && email.IndexOf("venancio") >= 0) return GADU;
            else if (email.IndexOf("bruno") >= 0 && email.IndexOf("krost") >= 0) return BKROST;
            else if (email.IndexOf("juliano") >= 0 && email.IndexOf("battisti") >= 0) return JULIANO;
            
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
