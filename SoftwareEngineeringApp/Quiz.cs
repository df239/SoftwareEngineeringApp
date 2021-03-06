﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace SoftwareEngineeringApp
{
    public static class Quiz
    {
        //Lists of questions of all levels
        public static List<Question> questionsLvl1 = new List<Question>();
        public static List<Question> questionsLvl2 = new List<Question>();
        public static List<Question> questionsLvl3 = new List<Question>();
        public static List<Question> questionsLvl4 = new List<Question>();

        public static Dictionary<string, int> highscores = new Dictionary<string, int>();
        public static string currentUsername;

        public static bool gameOver = false;

        public static bool highscoresLoaded = false;
        public static bool questionsLoaded = false;

        public static void LoadHighScoresFromFile()
        {
            string currentDirPath = Environment.CurrentDirectory;
            string newDirPath = Path.Combine(currentDirPath, "C_Who's_Sharper");
            string newFilePath = Path.Combine(currentDirPath, "C_Who's_Sharper", "highscores.txt");

            if (!Directory.Exists(newDirPath))
            {
                Directory.CreateDirectory(newDirPath);
            }
           
            if (!File.Exists(newFilePath))
            {
                File.Create(newFilePath).Close();
            }

            StreamReader reader = new StreamReader(newFilePath);            
            using (reader)
            {
                string line = reader.ReadLine();
                while(line != null)
                {
                    string[] pair = line.Split(' ');
                    highscores.Add(pair[0], int.Parse(pair[1]));
                    line = reader.ReadLine();
                }
            }

            highscoresLoaded = true;
        }

        public static void LoadQuestionsFromFile()
        {
            //code for loading
            string text = Properties.Resources.questions; //to be uploaded
            //these data will be loaded from file;
            string[] questions = text.Split('*');
            string[] splitter = { "\r\n" };

            int loop = 0;
            foreach (string q in questions)
            {
                string[] partsOfQuestion = q.Split(splitter,StringSplitOptions.RemoveEmptyEntries);

                string difficulty = partsOfQuestion[0];
                string questionWording = partsOfQuestion[1];
                string option1 = partsOfQuestion[2];
                string option2 = partsOfQuestion[3];
                string option3 = partsOfQuestion[4];
                string option4 = partsOfQuestion[5];
                char correctAnswer = char.Parse(partsOfQuestion[6]);

                loop++;

                Question question = new Question(questionWording, option1, option2, option3, option4, correctAnswer);

                switch (difficulty)
                {
                    case "1":
                        questionsLvl1.Add(question);
                        break;
                    case "2":
                        questionsLvl2.Add(question);
                        break;
                    case "3":
                        questionsLvl3.Add(question);
                        break;
                    case "4":
                        questionsLvl4.Add(question);
                        break;
                    default:
                        questionsLvl1.Add(question);
                        break;
                }
            }

            highscoresLoaded = true;
        }

        public static void SaveHighscoresToFile()
        {
            string currentDirPath = Environment.CurrentDirectory;
            string newDirPath = Path.Combine(currentDirPath, "C_Who's_Sharper");

            StreamWriter writer = new StreamWriter(Path.Combine(newDirPath,"highscores.txt"), false);
            using (writer)
            {
                foreach (var score in highscores)
                {
                    writer.WriteLineAsync(score.Key + " " + score.Value);
                }
            }            
        }

        private static void CreateDirectory(string dirPath)
        {
            Directory.CreateDirectory(dirPath);
            File.Create(Path.Combine(dirPath, "highscores.txt"));
        }
    }
}
