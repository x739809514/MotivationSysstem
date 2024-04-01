using System;
using UnityEngine;

namespace Core
{
    public class InputManager
    {
        private string defaultDataSavePath = "Resources/Input/default";
        private string customDataSavePath = "Resources/Input/custom";
        private InputData inputData;
        private bool activeInput;
        private string fileName;
        private Action<KeyCode> setKeyAction;
        //use for test
        private Action<KeyCode> displayAction;
        public static InputManager instance;

        public InputManager(InputData data,string name)
        {
            instance = this;
            inputData = data;
            fileName = name;
        }

        public void Update(float deltaTime)
        {
            inputData.AcceptInput();
            if(activeInput==false) return;
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                setKeyAction?.Invoke(key);
                displayAction?.Invoke(key);
            }
        }

        public void AddListener(Action<KeyCode> action)
        {
            setKeyAction += action;
        }
        
#region GetValueOfKey

        public bool GetKeyDown(string name)
        {
            return inputData.GetKeyDown(name);
        }

        public bool GetKeyDoubleDown(string name)
        {
            return inputData.GetkeyDoubleDown(name);
        }

        public float GetValue(string name)
        {
            return inputData.GetValueOfVK(name);
        }

        public float GetAxisValue(string name)
        {
            return inputData.GetValueOfAK(name);
        }

#endregion


#region SetKey

        public void SetKey(KeyCode keyCode, string name)
        {
            inputData.SetKey(keyCode,name);
        }

        public void SetValueKey(KeyCode keyCode, string name)
        {
            inputData.SetValueKey(keyCode,name);
        }

        public void SetAxisKey(KeyCode pos, KeyCode neg, string name)
        {
            inputData.SetAxisKey(pos,neg,name);
        }

        public void SetPosAxisKey(KeyCode keyCode, string name)
        {
            inputData.SetPosAxisKey(keyCode,name);
        }

        public void SetNegAxisKey(KeyCode keyCode, string name)
        {
            inputData.SetNegAxisKey(keyCode,name);
        }
#endregion


#region SetKeyEnable

        public void SetKeyEnable(string name, bool isEnable)
        {
            inputData.SetKeyEnable(name,isEnable);
        }

        public void SetValueKeyEnable(string name,bool isEnable)
        {
            inputData.SetValueKeyEnable(name,isEnable);
        }

        public void SetAxisKeyEnable(string name, bool isEnable)
        {
            inputData.SetAxisKeyEnable(name,isEnable);
        }

#endregion

#region Save

        public void SaveDefaultSetting()
        {
            inputData.SaveInputData(defaultDataSavePath+"/"+fileName);
        }

        public void LoadDefaultSetting()
        {
            inputData.LoadInputData(defaultDataSavePath+"/"+fileName);
        }


        public void SaveCustomSetting()
        {
            inputData.SaveInputData(customDataSavePath+"/"+fileName);
        }

        public void LoadCustomSetting()
        {
            inputData.LoadInputData(customDataSavePath+"/"+fileName);            
        }
#endregion


#region GetKey

        public KeyCode GetKey(string name)
        {
            return inputData.GetKeyObject(name).keyCode;
        }

        public KeyCode GetValueKey(string name)
        {
            return inputData.GetValueKeyObject(name).keyCode;
        }

        public KeyCode GetPosAxisKey(string name)
        {
            return inputData.GetAxisKeyObject(name).posKey;
        }

        public KeyCode GetNegAxisKey(string name)
        {
            return inputData.GetAxisKeyObject(name).negKey;
        }
        
#endregion
    }
    
}