#region License
//   
//  Copyright 2013 Steven Thuriot
//   
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//    http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//   
#endregion

namespace Nova.Shell.Library
{
    /// <summary>
    /// Action context constants
    /// </summary>
    public static class ActionContextConstants
    {
        public const string NodeId = "### NODE ID ###";
        public const string DialogBoxMessage = "### DIALOGBOX MESSAGE ###";
        public const string DialogBoxImage = "### DIALOGBOX IMAGE ###";
        public const string DialogBoxButtons = "### DIALOGBOX BUTTONS ###";
        public const string DialogBoxResult = "### DIALOGBOX RESULT ###";
        public const string StackHandle = "### STACK HANDLE ###";
        public const string TriggerValidation = "### TRIGGER VALIDATION ###";
        public const string SessionDialogBox = "### SESSION DIALOG BOX ###";

        internal const string CurrentViewConstant = "### CurrentSessionContentView ###";
        internal const string CreateNextViewConstant = "### CreateNextSessionContentView ###";
        internal const string ViewTypeConstant = "### ViewTypeConstant ###";
        internal const string ViewModelTypeConstant = "### ViewModelTypeConstant ###";
    }
}
