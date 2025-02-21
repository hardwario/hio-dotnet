using Blazored.LocalStorage;
using hio_dotnet.UI.BlazorComponents.RadzenLib.Common.Console.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.Services
{
    public class AutomatedCommandsService
    {
        private readonly ILocalStorageService localStorage;
        public AutomatedCommandsService(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }
        public List<AutomatedCommandsTab> AutomatedCommandsTabs { get; set; } = new List<AutomatedCommandsTab>();

        public event EventHandler<string> OnAutomatedCommandExecutedRequest;
        public event EventHandler<Tuple<string, string>> OnRunningCommand;
        public event EventHandler<string> OnRunningCommandStateChanged;

        public async Task LoadAutomatedCommandsTabs()
        {
            try
            {
                AutomatedCommandsTabs = await localStorage.GetItemAsync<List<AutomatedCommandsTab>>("AutomatedCommandsTabs") ?? new List<AutomatedCommandsTab>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (AutomatedCommandsTabs.Count == 0)
            {
                AutomatedCommandsTabs.Add(new AutomatedCommandsTab()
                {
                    Name = "Tab 1",
                    AutomatedCommands = new List<AutomatedCommand>()
                    {
                        new AutomatedCommand()
                        {
                            Command = "config show",
                            Description = "Description 1",
                            DelayAfter = 1000,
                            DelayBefore = 1000,
                            IsActive = true
                        },
                        new AutomatedCommand()
                        {
                            Command = "info show",
                            Description = "Description 2",
                            DelayAfter = 2000,
                            DelayBefore = 2000,
                            IsActive = false
                        }
                    }
                });
            }
        }

        public async Task ClearStates(string tabId)
        {
            var tab = AutomatedCommandsTabs.FirstOrDefault(x => x.Id == tabId);
            if (tab != null)
            {
                foreach (var command in tab.AutomatedCommands)
                {
                    command.State = AutomatedCommandStates.None;
                }
                try
                {
                    await localStorage.SetItemAsync("AutomatedCommandsTabs", AutomatedCommandsTabs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public async Task RunAutomation(string tabId)
        {
            var tab = AutomatedCommandsTabs.FirstOrDefault(x => x.Id == tabId);
            if (tab != null)
            {
                foreach (var command in tab.AutomatedCommands)
                {
                    if (!command.IsActive)
                        continue;

                    command.State = AutomatedCommandStates.WaitingBefore;
                    OnRunningCommandStateChanged?.Invoke(this, command.Id);
                    await Task.Delay(command.DelayBefore);
                    command.State = AutomatedCommandStates.Running;
                    OnRunningCommandStateChanged?.Invoke(this, command.Id);
                    OnRunningCommand?.Invoke(this, new Tuple<string, string>(command.Id, command.Command));
                    command.State = AutomatedCommandStates.WaitingAfter;
                    OnRunningCommandStateChanged?.Invoke(this, command.Id);
                    OnAutomatedCommandExecutedRequest?.Invoke(this, command.Command);
                    command.State = AutomatedCommandStates.Done;
                    OnRunningCommandStateChanged?.Invoke(this, command.Id);
                    await Task.Delay(command.DelayAfter);
                }
            }
        }

        public async Task AddNewCommand(string tabId, string command, int timeBefore = 0, int timeAfter = 0)
        {
            var tab = AutomatedCommandsTabs.FirstOrDefault(x => x.Id == tabId);
            if (tab != null)
            {
                tab.AutomatedCommands.Add(new AutomatedCommand()
                {
                    Command = command,
                    Description = "Description",
                    DelayAfter = timeAfter,
                    DelayBefore = timeBefore,
                    IsActive = true
                });

                try
                {
                    await localStorage.SetItemAsync("AutomatedCommandsTabs", AutomatedCommandsTabs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task AddNewTab()
        {
            AutomatedCommandsTabs.Add(new AutomatedCommandsTab()
            {
                Name = "Tab",
                AutomatedCommands = new List<AutomatedCommand>()
            });
            try
            {
                await localStorage.SetItemAsync("AutomatedCommandsTabs", AutomatedCommandsTabs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task RemoveTab(string tabId)
        {
            var tab = AutomatedCommandsTabs.FirstOrDefault(x => x.Id == tabId);
            if (tab != null)
            {
                AutomatedCommandsTabs.Remove(tab);
                try
                {
                    await localStorage.SetItemAsync("AutomatedCommandsTabs", AutomatedCommandsTabs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
