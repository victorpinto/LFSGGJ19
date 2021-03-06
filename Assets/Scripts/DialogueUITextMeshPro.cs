﻿ /*

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
        public Camera cam;
        public GameObject whosTalking;
        public string nameOfChar;

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

        void Awake ()
        {
            if (dialogueContainer != null)
            {
                dialogueContainer.SetActive(false);
            }

            lineText.gameObject.SetActive (false);
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            optionsContainer.gameObject.SetActive(false);

            foreach (var button in optionButtons)
            {
                button.gameObject.SetActive (false);
            }
            
            if (continuePrompt != null)
            {
                continuePrompt.SetActive(false);
            }
        }

        string ParseText(string p_input)
        {

            /* Step the first: Here we use the colon to separate the raw string into segments  */
            var n = p_input.Split(':').First();          // Name
            Debug.Log(n);
            var not_name = p_input.Split(':').Last();       // This is some dialogue that is spoken *expression_1_r*

            //var dialogue = not_name.Split('*')[0];          // This is some dialogue that is spoken
            //var expression = not_name.Split('*')[1];        // expression_1_r

            // !Beware! There are TWO asterisks, hence the string is split into THREE pieces,
            // with the third being an empty string. We want the SECOND.

            // Step the second, we make use of our bits of text
            // we set the speaking character's name
            if (n != string.Empty || n != null)
            {
                Debug.Log("name is not empty");
                SetName(n);
                return n;
            }
            else return string.Empty;

            // we check what our expression is or how we want to move our portrait
            //if (expression != string.Empty || expression != null)
            //{
            //    YourExpressionHandlingMethodHere(expression);

            //    // ALTERNATIVELY
            //    var motion_type = expression.Split('_')[0]; //expression
            //    var index = expression.Split('_')[1]; // 1
            //    var orientation = expression.Split('_')[2]; // r that is, right
            //    YourExpressionHandlingMethodHere(motion_type, index, orientation);
            //}

            //// Step the third, take care of your dialogue
            //if (expression != string.Empty || expression != null)
            //{
            //    return content.ToString();               // this the string you use for your actual dialogue!               
            //}
            //else
            //{
            //    return string.Empty;
            //}
        }

        void SetName(string name)
        {
            nameOfChar = name;
        }

        /// Show a line of dialogue, gradually
        public override IEnumerator RunLine(Yarn.Line line)
        {
            // Show the text
            lineText.gameObject.SetActive(true);
            //dialogueContainer.transform.position = cam.ScreenToWorldPoint(whosTalking.transform.position);

            string dialogueText = ParseText(line.text);

            if (textSpeed > 0.0f) {
                // Display the line one character at a time
                var stringBuilder = new StringBuilder();

                //dialogueContainer.transform.position = cam.ViewportToWorldPoint(GameObject.Find(nameOfChar).transform.position /* + new Vector3(180, 180, 0)*/);
                //dialogueContainer.transform.position = GameObject.Find(nameOfChar).transform.position + new Vector3(5, 7, -15);

                foreach (char c in line.text) {
                    stringBuilder.Append(c);
                    lineText.text = stringBuilder.ToString();

                    yield return new WaitForSeconds(textSpeed);
                }
            }
        
            else {
                 //Display the line immediately if textSpeed == 0
                lineText.text = line.text;
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

                whosTalking = GameObject.Find(nameOfChar);

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
