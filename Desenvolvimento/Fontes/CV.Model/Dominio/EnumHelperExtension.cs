using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CV.Model.Dominio
{
    public static class EnumHelperExtension
    {
        public static string Descricao(this Enum enumValue)
        {
            ResourceManager _resources = new ResourceManager("CV.Model.Resource.EnumDescription", System.Reflection.Assembly.GetExecutingAssembly());


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