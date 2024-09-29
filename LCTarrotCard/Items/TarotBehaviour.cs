using System;
using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LCTarrotCard.Cards;
using LCTarrotCard.Config;
using LCTarrotCard.Ressource;
using LCTarrotCard.Util;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

namespace LCTarrotCard.Items {
    public class TarotBehaviour : GrabbableObject {

        private int cardLeft = 10;

        private AudioSource itemAudio;
        private PlayerControllerB playerWhoDrew;
        private bool isDrawingCard;

        private GameObject currentCard;
        private Card currentCardProperties;

        private bool drawingCoroutinePlaying;
        
        private Vector3 originalCardScale;

        private readonly AnimationCurve controlCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f, .5f, .5f),
            new Keyframe(1f, 1f, 0f, 0f, .5f, .5f));

        public override void Start() {
            base.Start();
            itemAudio = gameObject.GetComponent<AudioSource>();
            originalCardScale = gameObject.transform.localScale;
            if (ConfigManager.DeckSize.Value > 0 && ConfigManager.DeckSize.Value <= 100) {
                cardLeft = ConfigManager.DeckSize.Value;
            }
            SetNumberOfCardsServerRpc(cardLeft);
        }

        public override void ItemActivate(bool used, bool buttonDown = true) {
            if (deactivated || !IsOwner || !buttonDown || isDrawingCard || !isHeld || playerHeldBy == null || cardLeft <= 0) return;
            isDrawingCard = true;
            DrawCardServerRpc(playerHeldBy.playerClientId);

        }

        public bool CanDrawCard() {
            return StartOfRound.Instance.shipHasLanded && playerHeldBy != null && StartOfRound.Instance.currentLevel.levelID != (int) Planet.Gordion;
        }

        public override void SetControlTipsForItem() {
            if (!ConfigManager.DisplayCardLeft.Value) {
                base.SetControlTipsForItem();
                return;
            }
            List<string> toolTips = new List<string>(itemProperties.toolTips);
            if (cardLeft > 0) toolTips.Add(cardLeft + " card" + (cardLeft == 1 ? "" : "s") +" left");
            HUDManager.Instance.ChangeControlTipMultiple(toolTips.ToArray(), true, itemProperties);
        }

        public void StartDrawingCard(int pulledCard, int randomSeed) {
            if (playerWhoDrew == null) {
                PluginLogger.Error("Trying to pull a card with a null player");
                DestroyAfterDrawing();
                return;
            }
            
            if (pulledCard < 0 || pulledCard >= AllCards.GetAllCardsAsList().Count) {
                PluginLogger.Error("Trying to pull a card with an invalid index");
                DestroyAfterDrawing();
                return;
            }
            
            if (drawingCoroutinePlaying) return;
            
            cardLeft--;
            PluginLogger.Debug("Card left : " + cardLeft);
            if (playerHeldBy.playerClientId == GameNetworkManager.Instance.localPlayerController.playerClientId) 
                SetControlTipsForItem();

            if (cardLeft > 0) {
                float thickness = cardLeft * 0.1f;
                Vector3 newScale = new Vector3(originalCardScale.x, originalCardScale.y * thickness, originalCardScale.z);
                gameObject.transform.localScale = newScale;
                originalScale = newScale;
            }
            else {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
            
            Type pulledCardType = AllCards.GetAllCardsAsList()[pulledCard];
            currentCard = Instantiate(Assets.SingleTarotCard, gameObject.transform);
            currentCard.transform.localEulerAngles += new Vector3(180, 0, 0);
            PluginLogger.Debug("Card with type " + pulledCardType.Name);
            currentCardProperties = (Card) Activator.CreateInstance(pulledCardType, currentCard, itemAudio);
            currentCardProperties.InitCard(new Random(randomSeed));
            
            StartCoroutine(DrawingCoroutine());
        }

        private IEnumerator DrawingCoroutine() {
            drawingCoroutinePlaying = true;
            StartCoroutine(currentCardProperties.CardPullingCoroutine());
            yield return new WaitForSeconds(0.5f);
            Vector3 initPos = currentCard.transform.localPosition;
            Vector3 endPos = currentCard.transform.localPosition + new Vector3(0.2f, 0f, 0);
            float delta = 0f;
            while (delta < 0.5f) {
                currentCard.transform.localPosition = Vector3.Lerp(initPos, endPos, controlCurve.Evaluate(delta * 2));
                delta += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1.1f);
            ExecuteEffectServerRpc();
            yield return new WaitForSeconds(1f);
            drawingCoroutinePlaying = false;
            EndDrawingCard();
        }

        public void EndDrawingCard() {
            if (drawingCoroutinePlaying) return;
            DestroyAfterDrawing();
        }

        private void DestroyAfterDrawing() {
            playerWhoDrew = null;
            if (currentCard) {
                Destroy(currentCard);
            }
            currentCardProperties = null;

            if (cardLeft <= 0) {
                DestroyServerRpc();
            }
            
            isDrawingCard = false;
        }

        private void DestroyLocalClient() {
            itemAudio.volume = 0;
            DestroyObjectInHand(playerHeldBy);
            deactivated = true;
        }
        
        [ServerRpc]
        public void DestroyServerRpc() {
            if (!IsHost) DestroyLocalClient();
            DestroyClientRpc();
        }
        
        [ClientRpc]
        public void DestroyClientRpc() {
            DestroyLocalClient();
        }

        [ServerRpc]
        public void DrawCardServerRpc(ulong playerWhoPulled) {
            Type pulledCard = AllCards.PullRandomCard();
            
            if (!CanDrawCard()) 
                pulledCard = typeof(FoolCard);
            
            int cardIndex = AllCards.GetAllCardsAsList().IndexOf(pulledCard);
            DrawCardClientRpc(playerWhoPulled, cardIndex, new Random().Next());
        }

        [ClientRpc]
        public void DrawCardClientRpc(ulong playerWhoPulled, int pulledCard, int randomSeed) {
            isDrawingCard = true;
            playerWhoDrew = StartOfRound.Instance.allPlayerScripts[playerWhoPulled];
            StartDrawingCard(pulledCard, randomSeed);
        }
        
        [ServerRpc]
        public void SetNumberOfCardsServerRpc(int number) {
            cardLeft = number;
            SetNumberOfCardsClientRpc(number);
        }

        [ClientRpc]
        public void SetNumberOfCardsClientRpc(int number) {
            cardLeft = number;
            if (cardLeft > 0) {
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                float thickness = cardLeft * 0.1f;
                Vector3 newScale = new Vector3(originalCardScale.x, originalCardScale.y * thickness,
                    originalCardScale.z);
                gameObject.transform.localScale = newScale;
                originalScale = newScale;
            }
            else {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                DestroyServerRpc();
            }
        }

        [ServerRpc]
        public void ExecuteEffectServerRpc() {
            if (!IsServer) return;
            string hint;
            try {
                hint = currentCardProperties.ExecuteEffect(playerWhoDrew);
            } catch (Exception e) {
                PluginLogger.Error("Error while executing card effect : " + e);
                ExecuteEffectClientRpc(string.Empty, true);
                return;
            }
            
            ExecuteEffectClientRpc(hint);
        }
        
        [ClientRpc]
        public void ExecuteEffectClientRpc(string hint, bool error = false) {
            if (!IsOwner) return;
            if (error) {
                HUDManager.Instance.DisplayTip("Error", "An error occurred while executing the card effect, please report to the developer");
                return;
            }

            if (!ConfigManager.DisplayDrawnCard.Value) return;
            
            if (ConfigManager.DisplayCardEffect.Value) {
                HUDManager.Instance.DisplayTip(currentCardProperties.GetCardName(), hint);
            }
            else {
                HUDManager.Instance.DisplayTip(currentCardProperties.GetCardName(), "You drew " + currentCardProperties.GetCardName());
            }
        }
    }
}