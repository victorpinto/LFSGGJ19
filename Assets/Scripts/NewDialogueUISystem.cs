
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TMPro;

namespace Yarn.Unity.Example
{
    // all we need here is RunLine, RunOptions, RunCommand and DialogueComplete coroutines 

    public class NewDialogueUISystem : Yarn.Unity.DialogueUIBehaviour
    {

        // this object containst the dialogia and the options 
        public GameObject dialogueContainer;

        //contains the ui that are the options 
        public GameObject optionsContainer;

        //the UI element that displays lines of dialogue 
        public TextMeshProUGUI lineText;

        // game object for continuing.. rethinking implementation... maybe a button
        public GameObject continuePrompt;

        /// A delegate (ie a function-stored-in-a-variable) that
        /// we call to tell the dialogue system about what option
        /// the user selected
        private Yarn.OptionChooser SetSelectedOption;

        /// How quickly to show the text, in seconds per character
        [Tooltip("How quickly to show the text, in seconds per character")]
        public float textSpeed = 0.025f;

        // the buttons that let the user choose an option 
        public List<Button> optionButtons;

        // temporarily disable controls when dialogue is active 
        public RectTransform gameControlsContainer;

        void Awake()
        {
            if (dialogueContainer != null)
            {
                dialogueContainer.SetActive(false);
            }

            lineText.gameObject.SetActive(false);
            optionsContainer.gameObject.SetActive(false);

            foreach (var button in optionButtons)
            {
                button.gameObject.SetActive(false);
            }

            if (continuePrompt != null)
            {
                continuePrompt.SetActive(false);
            }
        }

        // here is where we show a line of dialogue gradually 
        public override IEnumerator RunLine(Yarn.Line line)
        {
            //our string parsing 
            string speakerName = "";
            string lineTextDisplay = line.text;
            if (line.text.Contains(":"))
            { // if there's a ":" separator, then identify the first part as a speaker
                var splitLine = line.text.Split(new char[] { ':' }, 2); // but only split once
                speakerName = splitLine[0].Trim();
                lineTextDisplay = splitLine[1].Trim();

            }

            //display our dialogue without the speaker name 
            if (textSpeed > 0.0f)
            {
                // dislpay the line one character at a time
                var stringBuilder = new StringBuilder();

                bool earlyOut = false;
                yield return 0; // give time for previous input.anykeydown event to become false
                foreach (char c in lineTextDisplay)
                {
                    float timeWaited = 0f;
                    stringBuilder.Append(c);
                    lineText.text = stringBuilder.ToString();
                    while (timeWaited < textSpeed)
                    {
                        timeWaited += Time.deltaTime;
                        //early out / skip ahead 
                        if (Input.anyKeyDown)
                        {
                            lineText.text = lineTextDisplay;
                            earlyOut = true;

                        }
                        lineText.gameObject.SetActive(true);
                        Debug.Log("LineText display");
                        yield return 0;
                    }
                    if (earlyOut) { break; }
                }

                yield return 0;

            }
        }

        public override IEnumerator RunOptions(Yarn.Options optionsCollection, Yarn.OptionChooser optionChooser)
        {

            while (SetSelectedOption != null)
            {
                yield return null;
            }

            SetSelectedOption = optionChooser;
        }

        public void SetOption (int selectedOption)
        {
            // call the delegate to tell the diallgue system that we've selected an option 
            SetSelectedOption(selectedOption);

            //now remove the delegate so that the loop in RunOptions will exit 
            SetSelectedOption = null;

        }

        /// Run an internal command.
        public override IEnumerator RunCommand(Yarn.Command command)
        {
            // "Perform" the command
            Debug.Log("Command: " + command.text);

            yield break;
        }
        // run an internal command 
        public override IEnumerator DialogueStarted()
        {
            Debug.Log("Dialogue Starting ooya");
            // endble dialogue controlls 
            if (dialogueContainer != null)
            {
                dialogueContainer.SetActive(true);
            }

            yield break;

        }

        // called when the dialogue system has finished running 
        public override IEnumerator DialogueComplete()
        {
            Debug.Log("Dialogue Complete");

            // Hide the dialogue interface.
            if (dialogueContainer != null)
                dialogueContainer.SetActive(true);

            // Show the game controls.
            if (gameControlsContainer != null)
            {
                gameControlsContainer.gameObject.SetActive(true);
            }

            yield break;
        }

    }
}