using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CV.Model.Results;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace CV.Business.Library
{
	public abstract partial class BusinessBase
	{
		public List<ServiceResult> serviceResult = new List<ServiceResult>();

		protected void LimparValidacao()
		{
			serviceResult.Clear();
		}

		protected void ValidateService(object entity)
		{
			ValidationFactory.ResetCaches();
			Validator validator = ValidationFactory.CreateValidator(entity.GetType());
			ValidationResults results = validator.Validate(entity);
			AddValidationResults(results);
		}



		protected virtual void AddValidationResults(ValidationResults results)
		{
			ServiceResult singleResult;
			Message message;

			foreach (ValidationResult result in results)
			{
				singleResult = new ServiceResult();
				message = new Message();
				message.Field = result.Key;
				message.Description.Add(result.Message);
				singleResult.Messages.Add(message);
				singleResult.Success = false;
				serviceResult.Add(singleResult);
			}
		}

		public bool IsValid()
		{
			if (serviceResult != null)
			{
				foreach (var singleResult in serviceResult)
					if (!singleResult.Success)
						return false;
			}

			return true;
		}
	}
}