using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Utility
{
    public class PosAutoMaper
    {
        public static M Map<M,VM>(M model, VM viewModel)
        {

            foreach (var prop in model.GetType().GetProperties())
            {
                var result = viewModel.GetType().GetProperties().Where(n => n.Name == prop.Name).FirstOrDefault();
                if (result != null)
                {
                    prop.SetValue(model, result.GetValue(viewModel));
                }
            }
            return model;
        }
    }
}
