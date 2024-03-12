using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;

public class CacheckGuiAndApplicationSynchronizer(CacheckApplicationModel model, CacheckWindow window,
                ISimpleLogger simpleLogger)
                    : GuiAndApplicationSynchronizerBase<CacheckApplicationModel, CacheckWindow>(model, window,
                        simpleLogger);