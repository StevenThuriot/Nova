#region License

// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System.Linq;
using Nova.Base;
using System;

namespace Nova.Shell.Actions.MainWindow
{
    public class CloseSession : BaseAction<MainView, MainViewModel>
    {
        private SessionView _Session;

        public override bool Execute()
        {
            var id = ActionContext.GetValue<Guid>("PageID");

            _Session = ViewModel.Sessions.FirstOrDefault(x => x.ID == id);

            return _Session != null;
        }

        public override void ExecuteCompleted()
        {
            //TODO: Invoke and await Session.LeaveStep.
            ViewModel.Sessions.Remove(_Session);
        }
    }
}
