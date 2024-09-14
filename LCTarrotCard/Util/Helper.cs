using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace LCTarrotCard.Util {
    
    [HarmonyPatch]
    public class Helper {
        
        
        public static class Enemies {

            public static EnemyType Baboon;
            public static EnemyType Slime;
            public static EnemyType BushWolf;
            public static EnemyType Butler;
            public static EnemyType ButlerBees;
            public static EnemyType Centipede;
            public static EnemyType ClaySurgeon;
            public static EnemyType Crawler;
            public static EnemyType DocileLocust;
            public static EnemyType Manticoil;
            public static EnemyType DressGirl;
            public static EnemyType FlowerMan;
            public static EnemyType FlowerSnake;
            public static EnemyType ForestGiant;
            public static EnemyType HoarderBug;
            public static EnemyType Jester;
            public static EnemyType LassoMan;
            public static EnemyType Masked;
            public static EnemyType MouthDog;
            public static EnemyType Nutcracker;
            public static EnemyType Puffer;
            public static EnemyType RadMech;
            public static EnemyType RedLocust;
            public static EnemyType RedPill;
            public static EnemyType Spider;
            public static EnemyType SandWorm;
            public static EnemyType SpringMan;

            public static readonly List<EnemyType> AllEnemies = new List<EnemyType>();
            public static readonly List<EnemyType> SpawnableEnemies = new List<EnemyType>();

            [CanBeNull]
            public static EnemyType GetByName(string name) {
                return AllEnemies.FirstOrDefault(enemy => enemy.enemyName == name);
            }
        }
        
        public static void InitEnemies() {
            PluginLogger.Debug("InitEnemies");
            EnemyType[] enemyArray = Resources.FindObjectsOfTypeAll<EnemyType>().Concat(Object.FindObjectsByType<EnemyType>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)).ToArray();
            PluginLogger.Debug("Found " + enemyArray.Length + " enemies");
            foreach (EnemyType currentEnemy in enemyArray) {
                PluginLogger.Debug("Found enemy : " + currentEnemy.enemyName);
                if (!Enemies.AllEnemies.Contains(currentEnemy)) {
                    Enemies.AllEnemies.Add(currentEnemy);
                    if (currentEnemy.enemyName != "Lasso" || currentEnemy.enemyName != "Red pill")
                        Enemies.SpawnableEnemies.Add(currentEnemy);
                }
                
                
                
                switch (currentEnemy.enemyName) {
                    case "Baboon hawk":
                        Enemies.Baboon = currentEnemy;
                        break;
                    case "Blob":
                        Enemies.Slime = currentEnemy;
                        break;
                    case "Bush Wolf":
                        Enemies.BushWolf = currentEnemy;
                        break;
                    case "Butler":
                        Enemies.Butler = currentEnemy;
                        break;
                    case "Butler Bees":
                        Enemies.ButlerBees = currentEnemy;
                        break;
                    case "Centipede":
                        Enemies.Centipede = currentEnemy;
                        break;
                    case "Clay Surgeon":
                        Enemies.ClaySurgeon = currentEnemy;
                        break;
                    case "Crawler":
                        Enemies.Crawler = currentEnemy;
                        break;
                    case "Docile Locust Bees":
                        Enemies.DocileLocust = currentEnemy;
                        break;
                    case "Manticoil":
                        Enemies.Manticoil = currentEnemy;
                        break;
                    case "Girl":
                        Enemies.DressGirl = currentEnemy;
                        break;
                    case "Flowerman":
                        Enemies.FlowerMan = currentEnemy;
                        break;
                    case "Tulip Snake":
                        Enemies.FlowerSnake = currentEnemy;
                        break;
                    case "ForestGiant":
                        Enemies.ForestGiant = currentEnemy;
                        break;
                    case "Hoarding bug":
                        Enemies.HoarderBug = currentEnemy;
                        break;
                    case "Jester":
                        Enemies.Jester = currentEnemy;
                        break;
                    case "Lasso":
                        Enemies.LassoMan = currentEnemy;
                        break;
                    case "Masked":
                        Enemies.Masked = currentEnemy;
                        break;
                    case "MouthDog":
                        Enemies.MouthDog = currentEnemy;
                        break;
                    case "Nutcracker":
                        Enemies.Nutcracker = currentEnemy;
                        break;
                    case "Puffer":
                        Enemies.Puffer = currentEnemy;
                        break;
                    case "RadMech":
                        Enemies.RadMech = currentEnemy;
                        break;
                    case "Red Locust Bees":
                        Enemies.RedLocust = currentEnemy;
                        break;
                    case "Red pill":
                        Enemies.RedPill = currentEnemy;
                        break;
                    case "Bunker Spider":
                        Enemies.Spider = currentEnemy;
                        break;
                    case "Earth Leviathan":
                        Enemies.SandWorm = currentEnemy;
                        break;
                    case "Spring":
                        Enemies.SpringMan = currentEnemy;
                        break;
                }
            }
        }
        
        private static readonly System.Random _rng = new System.Random();  

        public static void Shuffle<T>(IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = _rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
        
        public static GameObject ClosestAINode(Vector3 position, bool inside) {
            GameObject[] nodes = inside ? GameObject.FindGameObjectsWithTag("AINode") : GameObject.FindGameObjectsWithTag("OutsideAINode");
            GameObject closest = null;
            float minDist = float.MaxValue;
            foreach (GameObject node in nodes) {
                float dist = Vector3.Distance(node.transform.position, position);
                if (dist < minDist) {
                    minDist = dist;
                    closest = node;
                }
            }
            return closest;
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void InitEnemiesPatch() {
            InitEnemies();
        }

    }
}