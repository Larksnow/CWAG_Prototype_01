using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[System.Serializable]
public class DialogueBranch
{
    public string condition;
    public string nextNodeID;
}

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string text;
    public List<DialogueBranch> branches;
    public string nextNodeID;
}

[System.Serializable]
public class NpcDialogue
{
    public string npcID;
    public string dayID;
    public List<DialogueNode> dialogue;
}

public class DialogueManager : MonoBehaviour
{
    public List<TextAsset> dialogueDataList;
    public OptionState optionState;
    public TextMeshProUGUI dialogueUI;
    private bool isTyping = false;
    [SerializeField] private float typeSpeed = 0.02f;
    private NpcDialogue currentDialogue;
    private DialogueNode currentNode;
    private string nextNodeID;
    public string currentText;

    private void Awake()
    {
        LoadDialogueData();
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isTyping)
            {
                StartNode(nextNodeID);
            }
        }
    }

    #region Load dialogue json from Addressable
    private void LoadDialogueData()
    {
        Addressables.LoadAssetsAsync<TextAsset>("dialogue", null).Completed += OnDialogueLoaded;
    }

    private void OnDialogueLoaded(AsyncOperationHandle<IList<TextAsset>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            dialogueDataList = new List<TextAsset>(handle.Result);
            ParseDialogue("A_Day1");
            StartNode("node1");
            Debug.Log("Dialogue data loaded successfully!");
        }
        else
        {
            Debug.LogError("Failed to load dialogue data.");
        }
    }
    #endregion

    public void ParseDialogue(string fileName)
    {
        TextAsset jsonFile = dialogueDataList.Find(file => file.name == fileName);
        if (jsonFile != null)
        {
            currentDialogue = JsonUtility.FromJson<NpcDialogue>(jsonFile.text);
            Debug.Log($"Loaded dialogue for NPC {currentDialogue.npcID} on {currentDialogue.dayID}");
        }
        else
        {
            Debug.LogError($"Dialogue with key {fileName} not found.");
        }
    }

    public void StartNode(string nodeID)
    {
        currentNode = currentDialogue.dialogue.Find(node => node.id == nodeID);
        if (currentNode != null)
        {
            DisplayNode();
        }
        else
        {
            Debug.LogError($"Node with ID {nodeID} not found.");
        }
    }

    public void DisplayNode()
    {
        if (isTyping){return;}
        Debug.Log($"{currentDialogue.npcID}: {currentNode.text}");
        StartCoroutine(TypeText(currentNode.text));

        // decide where to go in next node
        if (currentNode.branches != null && currentNode.branches.Count > 0)
        {
            foreach (var branch in currentNode.branches)
            {
                if (CheckCondition(branch.condition))
                {
                    nextNodeID = branch.nextNodeID;
                    return;
                }
            }
        }
        else if (!string.IsNullOrEmpty(currentNode.nextNodeID))
        {
            nextNodeID = currentNode.nextNodeID;
        }
        else
        {
            Debug.Log("Dialogue ended.");
            currentNode = null;
        }
    }

    bool CheckCondition(string condition)
    {
        switch (condition)
        {
            case "isOption1":
                return optionState.isOption1;
            case "isOption2":
                return optionState.isOption2;
            default:
                return true;
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        currentText = "";
        foreach (char letter in text.ToCharArray())
        {
            currentText += letter;
            dialogueUI.text = currentText;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }
}

