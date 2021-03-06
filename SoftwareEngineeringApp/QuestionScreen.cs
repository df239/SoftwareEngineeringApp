﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace SoftwareEngineeringApp
{
    public partial class QuestionScreen : Form
    {
        private int questionNumber = 1;
        private int userScore = 0;
        private static int time = 20;
        Question currentQuestion;
        private int gameDiff;
        Button pressedButton = new Button();

        private List<Question> usedQuestions = new List<Question>();
        Timer timer2 = new Timer();
        private bool correctAnswerGiven;

        public QuestionScreen(int gameDifficulty)
        {
            InitializeComponent();
           
            this.ActiveControl = null;
            this.gameDiff = gameDifficulty;
            userScore = 0;
            timer.Interval = 1000;
            ChooseQuestion(this.gameDiff, this.questionNumber);

            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Interval = 500;
        }

        private void QuestionScreen_FormClosing(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void DisplayQuestion(int difficulty)
        {
            SetTimer(20);
            timer.Start();

            switch (difficulty)
            {
                case 1:
                    currentQuestion = GetRandomQuestion(Quiz.questionsLvl1);
                    break;
                case 2:
                    currentQuestion = GetRandomQuestion(Quiz.questionsLvl2);
                    break;
                case 3:
                    currentQuestion = GetRandomQuestion(Quiz.questionsLvl3);
                    break;
                case 4:
                    currentQuestion = GetRandomQuestion(Quiz.questionsLvl4);
                    break;
            }

            questionNumber_label.Text = "Question " + questionNumber.ToString();
            question_label.Text = currentQuestion.QuestionWording;
            optionA_button.Text = currentQuestion.Options[0];
            optionB_button.Text = currentQuestion.Options[1];
            optionC_button.Text = currentQuestion.Options[2];
            optionD_button.Text = currentQuestion.Options[3];

            score_label.Text = "Score: " + userScore.ToString();
        }

        private Question GetRandomQuestion(List<Question> questionList)
        {
            Random random = new Random();
            int randomPos;
            Question selectedQuestion;
            do
            {
                randomPos = random.Next(0, questionList.Count);
                selectedQuestion = questionList[randomPos];
            }
            while (this.usedQuestions.Contains(selectedQuestion) == true);

            this.usedQuestions.Add(selectedQuestion);
            return selectedQuestion;
        }

        private void ChooseQuestion(int gameDifficulty, int questionNumber)
        {
            if (questionNumber <= 20)
            {
                if (gameDifficulty == 1 && questionNumber <= 10)
                {
                    DisplayQuestion(1);
                }
                else if ((gameDifficulty == 1 && questionNumber <= 20) || (gameDifficulty == 2 && questionNumber <= 10))
                {
                    DisplayQuestion(2);
                }
                else if ((gameDifficulty == 2 && questionNumber <= 20) || (gameDifficulty == 3 && questionNumber <= 10))
                {
                    DisplayQuestion(3);
                }
                else
                {
                    DisplayQuestion(4);
                }
            }
            else
            {
                WinGame();
            }
        }

        private void SetTimer(int seconds)
        {
            time = seconds;
        }

        private void optionA_button_Click(object sender, EventArgs e)
        {
            this.pressedButton = this.optionA_button;
            EvaluateAnswer('A');
            this.ActiveControl = null;  //unfocuses the button
        }

        private void optionB_button_Click(object sender, EventArgs e)
        {
            this.pressedButton = this.optionB_button;
            EvaluateAnswer('B');
            this.ActiveControl = null;  //unfocuses the button
        }

        private void optionC_button_Click(object sender, EventArgs e)
        {
            this.pressedButton = this.optionC_button;
            EvaluateAnswer('C');
            this.ActiveControl = null;  //unfocuses the button
        }

        private void optionD_button_Click(object sender, EventArgs e)
        {
            this.pressedButton = this.optionD_button;
            EvaluateAnswer('D');
            this.ActiveControl = null;  //unfocuses the button
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time--;
            timer_label.Text = time.ToString();

            if (time == 0)
            {
                AnsweredIncorrectly();
            }
        }

        private void AnsweredIncorrectly()
        {
            timer.Stop();
            ChangeButtonColour(this.pressedButton, false);
            PlaySound(false);
            timer2.Start();
        }

        private void AnsweredCorrectly()
        {
            timer.Stop();
            ChangeButtonColour(this.pressedButton, true);
            userScore += 5;
            questionNumber++;
            PlaySound(true);
            timer2.Start();
        }

        private void PlaySound(bool correctAnswer)
        {
            Stream stream = null;
            if (correctAnswer)
            {
                stream = Properties.Resources.correctBell;
            }
            else
            {
                stream = Properties.Resources.wrongAns;
            }

            SoundPlayer sound = new SoundPlayer(stream);
            sound.PlaySync();            
        }

        private void ChangeButtonColour(Button button, bool correctAnswer)
        {
            if (correctAnswer)
            {
                button.BackColor = Color.Green;
                button.Update();
            }
            else
            {
                button.BackColor = Color.Red;
                button.Update();

                if (this.currentQuestion.CorrectAnswer == 'A') this.ChangeButtonColour(this.optionA_button, true);
                else if (this.currentQuestion.CorrectAnswer == 'B') this.ChangeButtonColour(this.optionB_button, true);
                else if (this.currentQuestion.CorrectAnswer == 'C') this.ChangeButtonColour(this.optionC_button, true);
                else this.ChangeButtonColour(this.optionD_button, true);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.pressedButton.BackColor = SystemColors.Info;
            timer2.Stop();
            if (this.correctAnswerGiven)
            {
                ChooseQuestion(this.gameDiff, this.questionNumber);
            }
            else
            {
                gameOver();
            }

            EnableButtons();
        }

        private void EnableButtons()
        {
            this.optionA_button.Enabled = true;
            this.optionB_button.Enabled = true;
            this.optionC_button.Enabled = true;
            this.optionD_button.Enabled = true;
        }

        private void EvaluateAnswer(char answer)
        {
            this.correctAnswerGiven = currentQuestion.CheckAnswer(answer);
            if (this.correctAnswerGiven)
            {
                AnsweredCorrectly();
            }
            else
            {
                AnsweredIncorrectly();
            }
        }

        private void SaveScore()
        {
            Quiz.highscores[Quiz.currentUsername] = userScore;
        }

        private void gameOver()
        {
            SaveScore();
            this.Hide();
            endScreen endForm = new endScreen();
            endForm.ShowDialog();
            this.Close();
        }

        private void help5050button_Click(object sender, EventArgs e)
        {
            char[] allOptions = { 'A', 'B', 'C', 'D' };
            char[] enabledOptions = new char[2];
            char[] disabledOptions = new char[2];
            enabledOptions[0] = this.currentQuestion.CorrectAnswer;
            char secondAnswer;
            do
            {
                secondAnswer = GetRandomAnswer();
            }
            while (enabledOptions[0] == secondAnswer);
            enabledOptions[1] = secondAnswer;

            int wrongOptionCounter = 0;
            for(int i = 0; i < 4; i++)
            {
                if (!enabledOptions.Contains(allOptions[i]))
                {
                    disabledOptions[wrongOptionCounter] = allOptions[i];
                    wrongOptionCounter++;
                }
            }

            foreach (char option in disabledOptions)
            {
                if (option == 'A') this.optionA_button.Enabled = false;
                else if (option == 'B') this.optionB_button.Enabled = false;
                else if (option == 'C') this.optionC_button.Enabled = false;
                else if (option == 'D') this.optionD_button.Enabled = false;
            }
            
            help5050button.Enabled = false;
        }

        private char GetRandomAnswer()
        {
            Random rand = new Random();
            int randomAnswer = rand.Next(4);

            if (randomAnswer == 0) return 'A';
            else if (randomAnswer == 1) return 'B';
            else if (randomAnswer == 2) return 'C';
            else return 'D';
        }

        private void helpStopTimebutton_Click(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.helpStopTimebutton.Enabled = false;
        }

        private void helpNewQbutton_Click(object sender, EventArgs e)
        {
            if (Quiz.questionsLvl3.Contains(this.currentQuestion))
            {
                this.DisplayQuestion(2);
            }
            else if (Quiz.questionsLvl4.Contains(this.currentQuestion))
            {
                this.DisplayQuestion(3);
            }
            else
            {
                this.DisplayQuestion(1);
            }
            this.helpNewQbutton.Enabled = false;
        }

        public void WinGame()
        {
            SaveScore();
            this.Hide();
            victoryScreen winForm = new victoryScreen();
            winForm.ShowDialog();
            this.Close();           
        }

        private void OnMouseEnterButton1(object sender, EventArgs e)
        {
            //((Button)sender).BackColor = Color.LightSeaGreen;
            optionA_button.BackColor = Color.LightSeaGreen; //Changes colour of button to Light Sea Green
        }
        private void OnMouseLeaveButton1(object sender, EventArgs e)
        {
            optionA_button.BackColor = SystemColors.Info; //Reverts the colour to the original grey
        }
        private void OnMouseEnterButton2(object sender, EventArgs e) //Changes colour of button to Light Sea Green
        {
            optionB_button.BackColor = Color.LightSeaGreen; ;
        }
        private void OnMouseLeaveButton2(object sender, EventArgs e) //Reverts the colour to the original grey
        {
            optionB_button.BackColor = SystemColors.Info;
        }
        private void OnMouseEnterButton3(object sender, EventArgs e) //Changes colour of button to Light Sea Green
        {
            optionC_button.BackColor = Color.LightSeaGreen; ;
        }
        private void OnMouseLeaveButton3(object sender, EventArgs e) //Reverts the colour to the original grey
        {
            optionC_button.BackColor = SystemColors.Info;
        }
        private void OnMouseEnterButton4(object sender, EventArgs e) //Changes colour of button to Light Sea Green
        {
            optionD_button.BackColor = Color.LightSeaGreen; ;
        }
        private void OnMouseLeaveButton4(object sender, EventArgs e) //Reverts the colour to the original grey
        {
            optionD_button.BackColor = SystemColors.Info;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void QuestionScreen_Load(object sender, EventArgs e)
        {

        }
    }
}