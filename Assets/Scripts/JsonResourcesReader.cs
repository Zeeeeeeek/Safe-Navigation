using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonResourcesReader
{
    [System.Serializable]
    public class Content
    {
        public string type; 
        public List<string> elements;
        public string answer; 

        public Type GetTypeEnum()
        {
            // Mappa il valore stringa dell'attributo `type` con l'enum `Type`
            return type.ToLower() switch
            {
                "email" => Type.Email,
                "link" => Type.Link,
                _ => Type.Unknown // In caso di valori non validi
            };
        }

        public Answer GetAnswerEnum()
        {
            return answer.ToLower() switch
            {
                "spam" => Answer.SPAM,
                "safe" => Answer.SAFE,
                _ => Answer.UNKNOWN // In caso di valori non validi
            };
        }
    }

    public enum Type
    {
        Unknown, // Valore predefinito se il tipo è sconosciuto
        Email,
        Link
    }

    [System.Serializable]
    public class ContentWrapper
    {
        public List<Content> resources;
    }

    public IEnumerator ReadResources(System.Action<List<Content>> callback)
    {
        var path = Path.Combine(Application.streamingAssetsPath, "resources.json");

        if (path.Contains("://") || path.Contains(":///"))
        {
            using var request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonContent = request.downloadHandler.text;
                jsonContent = RemoveBom(jsonContent).Trim();
                var wrappedJson = "{ \"resources\": " + jsonContent + " }";

                try
                {
                    var contentWrapper = JsonUtility.FromJson<ContentWrapper>(wrappedJson);
                    // Applichiamo la deserializzazione manuale degli enum
                    foreach (var content in contentWrapper.resources)
                    {
                        content.answer = content.GetAnswerEnum().ToString();
                        content.type = content.GetTypeEnum().ToString();
                    }

                    callback?.Invoke(contentWrapper.resources);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON parsing failed: {e.Message}");
                    callback?.Invoke(new List<Content>());
                }
            }
            else
            {
                Debug.LogError($"Error loading file (WebGL): {request.error}");
                callback?.Invoke(new List<Content>());
            }
        }
        else
        {
            if (File.Exists(path))
            {
                var jsonContent = File.ReadAllText(path);
                var wrappedJson = "{ \"resources\": " + jsonContent + " }";

                try
                {
                    var contentWrapper = JsonUtility.FromJson<ContentWrapper>(wrappedJson);
                    foreach (var content in contentWrapper.resources)
                    {
                        content.answer = content.GetAnswerEnum().ToString();
                        content.type = content.GetTypeEnum().ToString();
                    }

                    callback?.Invoke(contentWrapper.resources);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON parsing failed: {e.Message}");
                    callback?.Invoke(new List<Content>());
                }
            }
            else
            {
                Debug.LogError($"File not found at path: {path}");
                callback?.Invoke(new List<Content>());
            }
        }
    }

    private static string RemoveBom(string jsonContent)
    {
        var BOM = System.Text.Encoding.UTF8.GetString(new byte[] { 0xEF, 0xBB, 0xBF });
        return jsonContent.StartsWith(BOM) ? jsonContent.Substring(BOM.Length) : jsonContent;
    }
}
