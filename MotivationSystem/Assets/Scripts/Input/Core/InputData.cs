using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

namespace Core
{
    [CreateAssetMenu(menuName = "RFrame/Input/InputSetting",fileName = "new input setting")]
    public class InputData : ScriptableObject
    {
        public List<Key> keys;
        public List<ValueKey> valueKeys;
        public List<AxisKey> axisKeys;


#region GetKey

        public Key GetKeyObject(string name)
        {
            return keys.Find(key => key.name == name);
        }

        public ValueKey GetValueKeyObject(string name)
        {
            return valueKeys.Find(key => key.name == name);
        }

        public AxisKey GetAxisKeyObject(string name)
        {
            return axisKeys.Find(key => key.name == name);
        }

#endregion


#region SetKey

        public void SetKey(KeyCode keyCode, string name)
        {
            var key = GetKeyObject(name);
            key.SetKey(keyCode);
        }

        public void SetValueKey(KeyCode keyCode, string name)
        {
            var valueKey = GetValueKeyObject(name);
            valueKey.SetKey(keyCode);
        }

        public void SetAxisKey(KeyCode pos, KeyCode neg, string name)
        {
            var axisKey = GetAxisKeyObject(name);
            axisKey.SetKey(pos, neg);
        }

        public void SetPosAxisKey(KeyCode pos, string name)
        {
            var axisKey = GetAxisKeyObject(name);
            axisKey.SetPosKey(pos);
        }

        public void SetNegAxisKey(KeyCode neg, string name)
        {
            var axisKey = GetAxisKeyObject(name);
            axisKey.SetNegKey(neg);
        }

#endregion


#region GetKeyDown

        public bool GetKeyDown(string name)
        {
            var key = GetKeyObject(name);
            return key.isDown;
        }

        public bool GetkeyDoubleDown(string name)
        {
            var key = GetKeyObject(name);
            return key.isDoubleDown;
        }

        public float GetValueOfVK(string name)
        {
            var valueKey = GetValueKeyObject(name);
            return valueKey.value;
        }

        public float GetValueOfAK(string name)
        {
            var axisKey = GetAxisKeyObject(name);
            return axisKey.value;
        }

#endregion


#region SetKeyEnable

        public void SetKeyEnable(string name, bool isEnable)
        {
            var key = GetKeyObject(name);
            key.SetEnable(isEnable);
        }

        public void SetValueKeyEnable(string name, bool isEnable)
        {
            var valueKey = GetValueKeyObject(name);
            valueKey.SetEnable(isEnable);
        }

        public void SetAxisKeyEnable(string name, bool isEnable)
        {
            var axisKey = GetAxisKeyObject(name);
            axisKey.SetEnable(isEnable);
        }

#endregion


#region UpdateKey

        public void AcceptInput()
        {
            UpdateKeys();
            UpdateValueKeys();
            UpdateAxisKeys();
        }

        private void UpdateKeys()
        {
            foreach (var key in keys)
            {
                if (key.enable == false) continue;
                key.isDown = false;
                key.isDoubleDown = false;
                switch (key.trigger)
                {
                    case KeyTrigger.Once:
                        if (Input.GetKeyDown(key.keyCode))
                        {
                            key.isDown = true;
                        }

                        break;
                    case KeyTrigger.Double:
                        if (key.acceptDoubleDown)
                        {
                            key.realInterval += Time.deltaTime;
                            if (key.realInterval > key.interval)
                            {
                                key.isDoubleDown = false;
                                key.acceptDoubleDown = false;
                                key.realInterval = 0f;
                            }
                            else
                            {
                                if (Input.GetKeyDown(key.keyCode))
                                {
                                    key.isDoubleDown = true;
                                    key.realInterval = 0f;
                                }
                                else if (Input.GetKeyUp(key.keyCode))
                                {
                                    key.acceptDoubleDown = false;
                                }
                            }
                        }
                        else
                        {
                            if (Input.GetKeyUp(key.keyCode))
                            {
                                key.acceptDoubleDown = true;
                                key.realInterval = 0f;
                            }
                        }

                        break;
                    case KeyTrigger.Continue:
                        if (Input.GetKey(key.keyCode))
                        {
                            key.isDown = true;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void UpdateValueKeys()
        {
            foreach (var key in valueKeys)
            {
                if (key.enable) continue;
                if (Input.GetKey(key.keyCode))
                {
                    key.value = Mathf.Clamp(key.value + key.addSpeed * Time.deltaTime, key.range.x, key.range.y);
                }
                else
                {
                    key.value = Mathf.Clamp(key.value - key.addSpeed * Time.deltaTime, key.range.x, key.range.y);
                }
            }
        }

        private void UpdateAxisKeys()
        {
            foreach (var key in axisKeys)
            {
                if (key.enable == false) continue;
                if (Input.GetKey(key.posKey))
                {
                    key.value = Mathf.Clamp(key.value + key.addSpeed * Time.deltaTime, key.range.x, key.range.y);
                }
                else if (Input.GetKey(key.negKey))
                {
                    key.value = Mathf.Clamp(key.value - key.addSpeed * Time.deltaTime, key.range.x, key.range.y);
                }
                else
                {
                    key.value = Mathf.Lerp(key.value, 0f, key.addSpeed * Time.deltaTime);
                    if (Mathf.Abs(key.value - 0) < 0.01f)
                    {
                        key.value = 0f;
                    }
                }
            }
        }

#endregion


#region Save

        public void SaveInputData(string path)
        {
            JsonData json = new JsonData();
            json["keys"] = new JsonData();
            foreach (var key in keys)
            {
                json["keys"][key.name] = key.keyCode.ToString();
            }

            json["valuekeys"] = new JsonData();
            foreach (var valueKey in valueKeys)
            {
                json["valuekeys"][valueKey.name] = valueKey.keyCode.ToString();
            }

            json["axiskeys"] = new JsonData();
            foreach (var axisKey in axisKeys)
            {
                json["axiskeys"][axisKey.name] = new JsonData();
                json["axiskeys"][axisKey.name]["pos"] = axisKey.posKey.ToString();
                json["axiskeys"][axisKey.name]["neg"] = axisKey.negKey.ToString();
            }

            string filePath = Application.dataPath + path;
            FileInfo file = new FileInfo(filePath);
            StreamWriter sw = file.CreateText();
            sw.WriteLine(json.ToJson());
            sw.Close();
            sw.Dispose();
        }
        
        public void LoadInputData(string path)
        {
            string filePath = Application.dataPath + path;
            if(!File.Exists(filePath)) return;
            string data = File.ReadAllText(filePath);
            JsonData json = JsonMapper.ToObject(data);

            foreach (var key in keys)
            {
                key.SetKey(String2Enum_KeyCode(json["keys"][key.name].ToString()));
            }

            foreach (var valueKey in valueKeys)
            {
                valueKey.SetKey(String2Enum_KeyCode(json["valuekeys"][valueKey.name].ToString()));
            }

            foreach (var axisKey in axisKeys)
            {
                axisKey.SetPosKey(String2Enum_KeyCode(json["axiskeys"][axisKey.name]["pos"].ToString()));
                axisKey.SetNegKey(String2Enum_KeyCode(json["axiskeys"][axisKey.name]["neg"].ToString()));
            }
        }

        private KeyCode String2Enum_KeyCode(string key)
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), key);
        }

#endregion
    }
}