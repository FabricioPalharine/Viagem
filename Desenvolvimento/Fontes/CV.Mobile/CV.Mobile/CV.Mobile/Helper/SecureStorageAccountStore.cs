using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Essentials;

namespace CV.Mobile.Helper
{
    public class SecureStorageAccountStore
    {
        public static async Task SaveAsync(Account account, string serviceId)
        {
            // Find existing accounts for the service
            var accounts = await FindAccountsForServiceAsync(serviceId);

            // Remove existing account with Id if exists
            accounts.RemoveAll(a => a.Username == account.Username);

            // Add account we are saving
            accounts.Add(account);

            // Serialize all the accounts to javascript
            var json = JsonConvert.SerializeObject(accounts);

            // Securely save the accounts for the given service
            await SecureStorage.SetAsync(serviceId, json);
        }

        public static async Task<List<Account>> FindAccountsForServiceAsync(string serviceId)
        {
            // Get the json for accounts for the service
            var json = await SecureStorage.GetAsync(serviceId);

            try
            {
                // Try to return deserialized list of accounts
                return JsonConvert.DeserializeObject<List<Account>>(json);
            }
            catch { }

            // If this fails, return an empty list
            return new List<Account>();
        }

        public static async Task MigrateAllAccountsAsync(string serviceId, IEnumerable<Account> accountStoreAccounts)
        {
            var wasMigrated = await SecureStorage.GetAsync($"XamarinAuthAccountStoreMigrated{serviceId}");

            if (wasMigrated == "1")
                return;

            await SecureStorage.SetAsync($"XamarinAuthAccountStoreMigrated{serviceId}", "1");

            // Just in case, look at existing 'new' accounts
            var accounts = await FindAccountsForServiceAsync(serviceId);

            foreach (var account in accountStoreAccounts)
            {

                // Check if the new storage already has this account
                // We don't want to overwrite it if it does
                if (accounts.Any(a => a.Username == account.Username))
                    continue;

                // Add the account we are migrating
                accounts.Add(account);
            }

            // Serialize all the accounts to javascript
            var json = JsonConvert.SerializeObject(accounts);

            // Securely save the accounts for the given service
            await SecureStorage.SetAsync(serviceId, json);
        }
    }
}
