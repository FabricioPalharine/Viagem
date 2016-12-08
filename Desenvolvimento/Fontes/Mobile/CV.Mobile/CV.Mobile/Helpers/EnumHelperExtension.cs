using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CV.Mobile.Helpers
{
    public static class EnumHelperExtension
    {
        public static string Descricao(this Enum enumValue)
        {
            ResourceManager _resources = new ResourceManager("CV.Mobile.Resource.EnumDescription", typeof(enumMoeda).GetTypeInfo().Assembly);


            string rk = String.Format("{0}_{1}", enumValue.GetType().Name, enumValue);
            string localizedDescription = _resources.GetString(rk);

            if (localizedDescription == null)
            {
                return enumValue.ToString();

            }
            else
                return localizedDescription;



        }
    }
}
