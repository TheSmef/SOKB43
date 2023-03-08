using Blazored.LocalStorage;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEB.Tests.Tests.Stubs
{
    public class LocalStorageStub : ILocalStorageService
    {
        public event EventHandler<ChangingEventArgs>? Changing;
        public event EventHandler<ChangedEventArgs>? Changed;

        public LocalStorageStub(string token) 
        {
            this.token = token;
        }

        private readonly string token;

        public ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            return new ValueTask<bool>(false);
        }

        public ValueTask<string> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string>(token);
        }

        public ValueTask<T> GetItemAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            return new ValueTask<T>();
        }

        public ValueTask<string> KeyAsync(int index, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string>(string.Empty);
        }

        public ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<IEnumerable<string>>(new List<string>());
        }

        public ValueTask<int> LengthAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<int>();
        }

        public ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = default)
        {
            return new ValueTask();
        }

        public ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            return new ValueTask();
        }

        public ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = default)
        {
            return new ValueTask();
        }

        public ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = default)
        {
            return new ValueTask();
        }
    }
}
