 /*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using TMPro;

namespace Yarn.Unity.Example {
    /// Displays dialogue lines to the player, and sends
    /// user choices back to the dialogue system.

    /** Note that this is just one way of presenting the
     * dialogue to the user. The only hard requirement
     * is that you provide the RunLine, RunOptions, RunCommand
     * and DialogueComplete coroutines; what they do is up to you.
     */
    public class DialogueUITextMeshPro : Yarn.Unity.DialogueUIBehaviour
    {

        /// The object that contains the dialogue and the options.
        /** This object will be enabled when conversation starts, and 
         * disabled when it ends.
         */
        public GameObject dialogueContainer;

        // contains the UI that are the options 
        public GameObject optionsContainer;

        /// The UI element that displays lines
        public TextMeshProUGUI lineText;

        /// A UI element that appears after lines have finished appearing
        public GameObject continuePrompt;

        // things for moving the dialogue around
        public Camera cam;

        // rect transform for moving the dialogue box
        public RectTransform dialogueMover;

        /// A delegate (ie a function-stored-in-a-variable) that
        /// we call to tell the dialogue system about what option
        /// the user selected
        private Yarn.OptionChooser SetSelectedOption;

        /// How quickly to show the text, in seconds per character
        [Tooltip("How quickly to show the text, in seconds per character")]
        public float textSpeed = 0.025f;

        /// The buttons that let the user choose an option
        //public List<Button> optionButtons;
        public List<Button> optionButtons;

        /// Make it possible to temporarily disable the controls when
        /// dialogue is active and to restore them when dialogue ends
        public RectTransform gameControlsContainer;

        void Awake()
        {
            if (dialogueContainer != null)
            {
                dialogueContainer.SetActive(false);
            }

            lineText.gameObject.SetActive(false);
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
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
        /// Show a line of dialogue, gradually
        public override IEnumerator RunLine(Yarn.Line line)
        {
            // Show the text
            Debug.Log("Test RunLine");


            //attempts to move the dialogue box 
            //dialogueContainer.transform.position = cam.ScreenToWorldPoint(whosTalking.transform.position);
            //dialogueContainer.transform.position = cam.ViewportToWorldPoint(GameObject.Find(nameOfChar).transform.position /* + new Vector3(180, 180, 0)*/);
            //dialogueContainer.transform.position = GameObject.Find(nameOfChar).transform.position + new Vector3(5, 7, -15);
           

            //our string parsing 
            string speakerName = "";
            string lineTextDisplay = line.text;
            if (line.text.Contains(":"))
            { // if there's a ":" separator, then identify the first part as a speaker
                var splitLine = line.text.Split(new char[] { ':' }, 2); // but only split once
                speakerName = splitLine[0].Trim();
                lineTextDisplay = splitLine[1].Trim();

            }

            // moving the dialogue stuff 
            Debug.Log(speakerName);

            GameObject[] npcList = GameObject.FindGameObjectsWithTag("NPC");


            for (int i = 0; i < npcList.Length; i++)
            {

                NPC current = npcList[i].GetComponent<NPC>();
                Debug.Log("current: " + current.characterName);
                if (current.characterName == speakerName)
                {
                    dialogueMover.position = current.transform.position + new Vector3(0, 0, 0);
                    Debug.Log("dialogue moved!");
                }
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
                        yield return 0;
                    }
                    if (earlyOut) { break; }
                }
            }
            else
            {
                for (int i = 0; i < npcList.Length; i++)
                {

                    NPC current = npcList[i].GetComponent<NPC>();
                    Debug.Log("current: " + current.characterName);
                    if (current.characterName == speakerName)
                    {
                        dialogueMover.position = current.transform.position + new Vector3(0, 0, 0);
                        Debug.Log("dialogue moved!");
                    }
                }
                //display the line immediatly if textSpeed == 0 
                lineText.text = lineTextDisplay;


                //Debug.Log("Display: " + lineTextDisplay);
               
            }


            // Show the 'press any key' prompt when done, if we have one
            if (continuePrompt != null)
            {
                continuePrompt.SetActive(true);
            }

            // Wait for any user input
            while (Input.anyKeyDown == false)
            {
                yield return null;
            }

            // Hide the text and prompt
            lineText.gameObject.SetActive (false);

            if (continuePrompt != null)
            {
                continuePrompt.SetActive(false);
            }

        }

        /// Show a list of options, and wait for the player to make a selection.
        public override IEnumerator RunOptions (Yarn.Options optionsCollection, Yarn.OptionChooser optionChooser)
        {
            // Do a little bit of safety checking
            if (optionsCollection.options.Count > optionButtons.Count) {
                Debug.LogWarning("There are more options to present than there are" +
                                 "buttons to present them in. This will cause problems.");
            }

            //set the options container to active
            optionsContainer.gameObject.SetActive(true);

            // Display each option in a button, and make it visible
            int i = 0;
            foreach (var optionString in optionsCollection.options) {
                optionButtons [i].gameObject.SetActive (true);
                optionButtons [i].GetComponentInChildren<TextMeshProUGUI> ().text = optionString;
                i++;
            }

            // Record that we're using it
            SetSelectedOption = optionChooser;

            // Wait until the chooser has been used and then removed (see SetOption below)
            while (SetSelectedOption != null) {
                yield return null;
            }

            // Hide all the buttons
            foreach (var button in optionButtons) {
                button.gameObject.SetActive (false);
            }
        }

        /// Called by buttons to make a selection.
        public void SetOption (int selectedOption)
        {

            // Call the delegate to tell the dialogue system that we've
            // selected an option.
            SetSelectedOption (selectedOption);

            // Now remove the delegate so that the loop in RunOptions will exit
            SetSelectedOption = null; 
        }

        /// Run an internal command.
        public override IEnumerator RunCommand (Yarn.Command command)
        {
            // "Perform" the command
            Debug.Log ("Command: " + command.text);

            yield break;
        }

        /// Called when the dialogue system has started running.
        public override IEnumerator DialogueStarted ()
        {
            Debug.Log ("Dialogue starting!");

            // Enable the dialogue controls.
            if (dialogueContainer != null)
            {
                dialogueContainer.SetActive(true);


            }

            // Hide the game controls.
            if (gameControlsContainer != null) {
                gameControlsContainer.gameObject.SetActive(false);
            }

            yield break;
        }

        /// Called when the dialogue system has finished running.
        public override IEnumerator DialogueComplete ()
        {
            Debug.Log ("Complete!");
            // tried to make a shortcut to end dialogue... didnt work
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    dialogueContainer.SetActive(false);
            //}
            // Hide the dialogue interface.
            if (dialogueContainer != null)
                dialogueContainer.SetActive(false);

            // Show the game controls.
            if (gameControlsContainer != null) {
                gameControlsContainer.gameObject.SetActive(true);
            }

            yield break;
        }

    }

}
