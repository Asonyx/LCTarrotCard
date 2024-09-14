using System;
using System.Collections;
using GameNetcodeStuff;
using LCTarrotCard.Cards;
using LCTarrotCard.Ressource;
using Unity.Netcode;
using UnityEngine;

namespace LCTarrotCard.Items {
    public class TarotBehaviour : GrabbableObject {

        private int cardLeft = 10;

        private AudioSource itemAudio;
        private PlayerControllerB playerWhoDrew;
        private bool isDrawingCard;

        private GameObject currentCard;
        private Card currentCardProperties;

        private bool drawingCoroutinePlaying;

        private readonly AnimationCurve controlCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f, .5f, .5f),
            new Keyframe(1f, 1f, 0f, 0f, .5f, .5f));

        public override void Start() {
            base.Start();
            itemAudio = gameObject.GetComponent<AudioSource>();
        }

        public override void ItemActivate(bool used, bool buttonDown = true) {
            if (deactivated || !StartOfRound.Instance.shipHasLanded || !IsOwner || !buttonDown || isDrawingCard || !isHeld || playerHeldBy == null || cardLeft <= 0) return;
            isDrawingCard = true;
            DrawCardServerRpc(playerHeldBy.playerClientId);

        }

        public void StartDrawingCard() {
            if (playerWhoDrew == null) {
                PluginLogger.Error("Trying to pull a card with a null player");
                DestroyAfterDrawing();
                return;
            }

            if (drawingCoroutinePlaying) return;
            
            currentCard = Instantiate(Assets.SingleTarotCard, gameObject.transform);
            currentCard.transform.localEulerAngles += new Vector3(180, 0, 0);
            Type t = AllCards.PullRandomCard();
            PluginLogger.Debug("Card with type " + t.Name);
            currentCardProperties = (Card) Activator.CreateInstance(t, currentCard, itemAudio);
            currentCardProperties.InitCard();
            
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
            currentCardProperties.ExecuteEffect(playerWhoDrew);
            yield return new WaitForSeconds(1f);
            drawingCoroutinePlaying = false;
            EndDrawingCard();
        }

        public void EndDrawingCard() {
            if (drawingCoroutinePlaying) return;
            DestroyAfterDrawing();
        }

        private void DestroyAfterDrawing() {
            cardLeft--;
            PluginLogger.Debug("Card left : " + cardLeft);
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
            DrawCardClientRpc(playerWhoPulled);
        }

        [ClientRpc]
        public void DrawCardClientRpc(ulong playerWhoPulled) {
            isDrawingCard = true;
            playerWhoDrew = StartOfRound.Instance.allPlayerScripts[playerWhoPulled];
            if (playerWhoDrew == null) PluginLogger.Error("Player is null when drawing card");
            StartDrawingCard();
        }
    }
}