﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Commands;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Application {
    public class CacheckApplication : ApplicationBase<IGuiAndApplicationSynchronizer<ICacheckApplicationModel>, ICacheckApplicationModel>, IConsole {
        public ICacheckHandlers Handlers { get; private set; }
        public ICacheckCommands Commands { get; private set; }
        public ITashHandler<ICacheckApplicationModel> TashHandler { get; private set; }

        private readonly ITashAccessor vTashAccessor;
        private readonly ISimpleLogger vSimpleLogger;
        private readonly ILogConfiguration vLogConfiguration;

        public CacheckApplication(IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
            IGuiAndApplicationSynchronizer<ICacheckApplicationModel> guiAndApplicationSynchronizer, ICacheckApplicationModel model,
            ITashAccessor tashAccessor, ISimpleLogger simpleLogger, ILogConfiguration logConfiguration
            ) : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model) {
            vTashAccessor = tashAccessor;
            vSimpleLogger = simpleLogger;
            vLogConfiguration = logConfiguration;
        }

        protected override async Task EnableOrDisableButtonsAsync() {
            await Task.CompletedTask;
        }

        protected override void CreateCommandsAndHandlers() {
            Handlers = new CacheckHandlers {
                SummaryTextHandler = new ConsoleOutputTextHandler(Model, this, m => m.Summary),
                AverageTextHandler = new ConsoleOutputTextHandler(Model, this, m => m.Average),
                LogTextHandler = new ConsoleOutputTextHandler(Model, this, m => m.Log),
            };
            Commands = new CacheckCommands();

            var selectors = new Dictionary<string, ISelector>();

            var communicator = new TashCommunicatorBase<ICacheckApplicationModel>(vTashAccessor, vSimpleLogger, vLogConfiguration);
            var selectorHandler = new TashSelectorHandler(Handlers, vSimpleLogger, communicator, selectors);
            var verifyAndSetHandler = new TashVerifyAndSetHandler(Handlers, vSimpleLogger, selectorHandler, communicator, selectors);
            TashHandler = new TashHandler(vTashAccessor, vSimpleLogger, vLogConfiguration, ButtonNameToCommandMapper, ToggleButtonNameToHandlerMapper, this, verifyAndSetHandler, selectorHandler, communicator);
        }

        public ITashTaskHandlingStatus<ICacheckApplicationModel> CreateTashTaskHandlingStatus() {
            return new TashTaskHandlingStatus<ICacheckApplicationModel>(Model, Process.GetCurrentProcess().Id);
        }

        public async Task WriteLineAsync(ConsoleOutputType outputType) {
            await WriteLineAsync(outputType, "");
        }

        public async Task WriteLineAsync(ConsoleOutputType outputType, string s) {
            ITextBox textBox;
            ISimpleTextHandler textHandler;
            switch (outputType) {
                case ConsoleOutputType.Log:
                    textBox = Model.Log;
                    textHandler = Handlers.LogTextHandler;
                    break;
                case ConsoleOutputType.Summary:
                    textBox = Model.Summary;
                    textHandler = Handlers.SummaryTextHandler;
                    break;
                case ConsoleOutputType.Average:
                    textBox = Model.Average;
                    textHandler = Handlers.AverageTextHandler;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outputType));
            }
            s = textBox.Text + (string.IsNullOrEmpty(textBox.Text) ? "" : "\r\n") + s;
            await textHandler.TextChangedAsync(s);
        }
    }
}
