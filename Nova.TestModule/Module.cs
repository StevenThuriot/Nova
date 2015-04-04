using System;
using Nova.Library;
using Nova.Shell.Library;

namespace Nova.TestModule
{
    /// <summary>
    /// Test Module
    /// </summary>
    public class Module : IModule
    {
        public static Guid Step1Id = Guid.NewGuid();
        public static Guid Step2Id = Guid.NewGuid();

        /// <summary>
        /// Configures the module.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(IModuleBuilder builder)
        {
            var seed = (int) DateTime.Now.Ticks;
            var random = new Random(seed);

            var randomRanking = random.Next(1, 100);

            builder.SetModuleTitle("Module #" + randomRanking)
                   .SetModuleRanking(randomRanking)

                   .AddNavigation<TestPage, TestPageViewModel>(Step1Id, rank: 10, parameters: ActionContextEntry.Create("this is a string", "test", false))

                   .AddNavigation<TestPage2, TestPage2ViewModel>(Step2Id, "Page #" + random.Next(1, 100))
                   .AddNavigation<TestPage2, TestPage2ViewModel>("Page #" + random.Next(1, 100))
                   .AddNavigation<TestPage2, TestPage2ViewModel>("Page #" + random.Next(1, 100))
                   .AddNavigation<TestPage2, TestPage2ViewModel>("Page #" + random.Next(1, 100))
                   .AddNavigation<TestPage2, TestPage2ViewModel>("Page #" + random.Next(1, 100))
                  /*
                   .AddNavigation("Multistep", x => x.AddStep<TestPage, TestPageViewModel>(Step1Id)
                                                     .AddStep<TestPage2, TestPage2ViewModel>()) */.AsStartup();
        }
    }
}
