﻿#region License

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

using Nova.Library.Actions;

namespace Nova.Shell.Actions.Session
{
    public class SessionLeaveStep : LeaveAction<SessionView, SessionViewModel>
    {
        public override bool Leave()
        {
            if (!ViewModel.IsSessionValid())
            {
                //Todo: Handle error state.
            }

            //if (!ViewModel.IsDirty)
            //{
            //    //Todo: Handle dirty state.
            //}

            return base.Leave();
        }
    }
}
