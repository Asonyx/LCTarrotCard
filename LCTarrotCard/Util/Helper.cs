using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LCTarrotCard.Util {
    
    [HarmonyPatch]
    public class Helper {
        
        
        /// <summary>
        /// For easy access to enemy types without having to tidiously search them every time
        /// </summary>
        public static class Enemies { // Not up to date, made in v56

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
                    default:
                        PluginLogger.Info("Unhandled enemy : " + currentEnemy.enemyName);
                        break;
                }
            }
        }
        
        private static readonly System.Random Rng = new System.Random();  
        
        /// <summary>
        /// Shuffle the list using Fisher-Yates algorithm
        /// Note : this will NOT copy the list, it will shuffle the original list (returns nothing)
        /// </summary>
        /// <param name="list">The list to shuffle</param>

        public static void Shuffle<T>(IList<T> list) {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = Rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }

        /// <summary>
        /// Get all the AI nodes inside the factory (or mension/mine)
        /// Warning : can return node that are inside some walls
        /// </summary>
        /// <returns>An array of all the AI nodes inside the factory</returns>
        public static GameObject[] GetAllInsideAINodes() {
            return GameObject.FindGameObjectsWithTag("AINode");
        }
        
        /// <summary>
        /// Get all the AI nodes of the map, outside
        /// </summary>
        /// <returns>The AI nodes outside</returns>
        public static GameObject[] GetAllOutsideAINodes() {
            return GameObject.FindGameObjectsWithTag("OutsideAINode");
        }
        
        /// <summary>
        /// Return the vector position of a random AI node inside or outside
        /// </summary>
        /// <param name="inside">True if the node should be inside, false otherwise</param>
        /// <returns>The position of the randomly chosen node</returns>
        public static Vector3 GetRandomAINodePosition(bool inside = true) {
            GameObject[] nodes = inside ? GetAllInsideAINodes() : GetAllOutsideAINodes();
            if (nodes.Length == 0) return Vector3.zero;
            return nodes[Random.Range(0, nodes.Length)].transform.position;
        }
        
        /// <summary>
        /// Return a list of all the enemy vents (spawning points) inside the map
        /// </summary>
        /// <returns>The enemy vents inside the map</returns>
        
        public static EnemyVent[] GetAllEnemyVents() {
            return Object.FindObjectsOfType<EnemyVent>();
        }

        /// <summary>
        /// Return a random spawn location for an enemy, either inside or outside the factory
        /// </summary>
        /// <param name="inside">Whether the spawn location should be inside the factory</param>
        /// <param name="useVentsInstead">Whether to use enemy vents instead of AI nodes</param>
        /// <returns>The random spawn location, or null if none is found</returns>
        [CanBeNull]
        public static Transform GetRandomSpawnLocation(bool inside = true, bool useVentsInstead = true) {
            if (inside) {
                EnemyVent[] vents = GetAllEnemyVents();
                if (vents.Length > 0 && useVentsInstead) {
                    return vents[Rng.Next(vents.Length)].transform;
                }
                GameObject[] nodes = GetAllInsideAINodes();
                if (nodes.Length == 0) return null;
                return nodes[Rng.Next(nodes.Length)].transform;
            }
            GameObject[] nodesOutside = GetAllOutsideAINodes();
            if (nodesOutside.Length == 0) return null;
            return nodesOutside[Rng.Next(nodesOutside.Length)].transform;
        }
        
        /// <summary>
        /// Find the closest AI node to a given position
        /// </summary>
        /// <param name="position">The position to find the closest node to</param>
        /// <param name="inside">True if the node should be inside, false otherwise</param>
        /// <returns>The closest AI node, or null if none is found</returns>
        public static GameObject ClosestAINode(Vector3 position, bool inside) {
            GameObject[] nodes = inside ? GameObject.FindGameObjectsWithTag("AINode") : GameObject.FindGameObjectsWithTag("OutsideAINode");
            GameObject closest = null;
            float minDist = float.MaxValue;
            foreach (GameObject node in nodes) {
                float dist = Vector3.Distance(node.transform.position, position);
                if (dist >= minDist) continue;
                minDist = dist;
                closest = node;
            }
            return closest;
        }

        /// <summary>
        /// Convert a dictionary to a string
        /// </summary>
        /// <param name="dict">The dictionary to convert</param>
        /// <returns>The string representation of the dictionary</returns>
        public static string DictionnaryToString<K, V>(Dictionary<K, V> dict) {
            return string.Join(", ", dict.Select(x => $"{x.Key}: {x.Value}"));
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void InitEnemiesPatch() {
            InitEnemies();
        }

    }
}