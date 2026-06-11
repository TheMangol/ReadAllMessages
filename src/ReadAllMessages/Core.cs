using System;
using System.Collections;
using System.Runtime.CompilerServices;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.UI.Phone.Messages;
#else
using ScheduleOne.DevUtilities;
using ScheduleOne.Messaging;
using ScheduleOne.UI.Phone.Messages;
#endif

[assembly: MelonInfo(typeof(ReadAllMessages.Core), "ReadAllMessages", "1.1.4", "Mangol")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ReadAllMessages
{
    /// <summary>
    /// Adds a "Read All" button to the phone Messages app.
    /// The button marks every conversation as read - nothing else.
    /// Fallback: the F8 hotkey does the same.
    /// </summary>
    public class Core : MelonMod
    {
        private const string ButtonName = "ReadAllButton";
        private bool legacyInputBroken;
        private bool newInputBroken;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // Game scenes where the phone exists
            if (sceneName == "Main" || sceneName == "Tutorial")
                MelonCoroutines.Start(SetupWhenReady());
        }

        public override void OnUpdate()
        {
            bool pressed = false;

            // The game uses the new InputSystem; legacy Input may be stripped.
            // Try both, disabling only the path that actually throws.
            if (!legacyInputBroken)
            {
                try
                {
                    pressed = LegacyF8Pressed();
                }
                catch
                {
                    legacyInputBroken = true;
                }
            }

            if (!pressed && !newInputBroken)
            {
                try
                {
                    pressed = NewInputF8Pressed();
                }
                catch
                {
                    newInputBroken = true;
                }
            }

            if (pressed)
            {
                try
                {
                    MarkAllRead();
                }
                catch (Exception e)
                {
                    MelonLogger.Error($"F8 mark-all-read failed: {e}");
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool LegacyF8Pressed()
        {
            return Input.GetKeyDown(KeyCode.F8);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool NewInputF8Pressed()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard is not null && keyboard.f8Key.wasPressedThisFrame;
        }

        private IEnumerator SetupWhenReady()
        {
            // Wait until the game creates MessagesApp (after the save loads)
            float waited = 0f;
            while (waited < 120f)
            {
                MessagesApp app = GetMessagesApp();
                if (app is not null && app.homePage is not null)
                {
                    try
                    {
                        CreateButton(app);
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Error($"Failed to create the Read All button: {e}");
                    }
                    yield break;
                }

                yield return new WaitForSeconds(0.5f);
                waited += 0.5f;
            }

            MelonLogger.Warning("MessagesApp not found within 120 seconds - button not added (F8 still works).");
        }

        private static MessagesApp GetMessagesApp()
        {
            try
            {
                MessagesApp app = GetAppViaSingleton();
                if (app is not null)
                    return app;
            }
            catch
            {
                // fall through
            }

            try
            {
                return GetAppViaFind();
            }
            catch
            {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static MessagesApp GetAppViaSingleton()
        {
            if (PlayerSingleton<MessagesApp>.InstanceExists)
                return PlayerSingleton<MessagesApp>.Instance;
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static MessagesApp GetAppViaFind()
        {
            return UnityEngine.Object.FindObjectOfType<MessagesApp>(true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ScanTree(Transform node, ref bool exists, ref Transform titleNode)
        {
            if (node.name == ButtonName)
            {
                exists = true;
                return;
            }

            if (titleNode is null)
            {
                Text text = node.GetComponent<Text>();
                if (text is not null && text.text == "Messages")
                    titleNode = node;
            }

            int count = node.childCount;
            for (int i = 0; i < count; i++)
                ScanTree(node.GetChild(i), ref exists, ref titleNode);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DumpTree(Transform node, int depth)
        {
            if (depth > 5)
                return;

            string label = node.name;
            Text text = node.GetComponent<Text>();
            if (text is not null)
                label += $" [Text:'{text.text}']";
            if (!node.gameObject.activeSelf)
                label += " (inactive)";
            MelonLogger.Msg(new string(' ', depth * 2) + label);

            int count = node.childCount;
            for (int i = 0; i < count; i++)
                DumpTree(node.GetChild(i), depth + 1);
        }

        private static void CreateButton(MessagesApp app)
        {
            Transform home = app.homePage.transform;

            // Walk up to the app root so we can scan the whole Messages app UI
            Transform root = home;
            for (int i = 0; i < 3 && root.parent is not null; i++)
                root = root.parent;

            // One BFS pass: detect an existing button + find the header title
            bool exists = false;
            Transform titleNode = null;
            ScanTree(root, ref exists, ref titleNode);

            if (exists)
                return;

            DumpTree(root, 0);

            // Host: the header bar (parent of the "Messages" title) when found,
            // otherwise fall back to the home page (proven visible in v1.1.0).
            Transform host;
            bool inHeader = titleNode is not null && titleNode.parent is not null;
            if (inHeader)
            {
                host = titleNode.parent;
                MelonLogger.Msg($"Header found: '{host.name}' (via title '{titleNode.name}')");
            }
            else
            {
                host = home;
                MelonLogger.Msg("Header not found - falling back to bottom-right of home page.");
            }

            // --- Button root ---
            GameObject go = new GameObject(ButtonName);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetParent(host, false);

            if (inHeader)
            {
                // Header band at the top of the page: the "Messages" title
                // center sits 72 units below the page's top edge
                // (measured from the dumped hierarchy + screenshot).
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 0.5f);
                rect.anchoredPosition = new Vector2(-16f, -72f);
            }
            else
            {
                // Bottom-right of the conversation list (v1.1.0 placement)
                rect.anchorMin = new Vector2(1f, 0f);
                rect.anchorMax = new Vector2(1f, 0f);
                rect.pivot = new Vector2(1f, 0f);
                rect.anchoredPosition = new Vector2(-12f, 12f);
            }
            rect.sizeDelta = new Vector2(180f, 48f);

            // Ignore any layout group on the host and render above siblings
            LayoutElement layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;
            go.transform.SetAsLastSibling();

            Image image = go.AddComponent<Image>();
            image.color = new Color(0.17f, 0.45f, 0.85f, 0.95f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = image;

            // --- Label ---
            GameObject labelGo = new GameObject("Label");
            RectTransform labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.SetParent(go.transform, false);
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = new Vector2(0f, 0f);
            labelRect.offsetMax = new Vector2(0f, 0f);

            Text label = labelGo.AddComponent<Text>();
            label.text = "Read All";
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(1f, 1f, 1f, 1f);
            label.fontSize = 21;
            label.fontStyle = FontStyle.Bold;
            label.raycastTarget = false;

            Font font = ResolveFont(app);
            if (font is not null)
                label.font = font;

            // --- Click handler (works on both Il2Cpp and Mono) ---
#if IL2CPP
            button.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>((Action)MarkAllRead));
#else
            button.onClick.AddListener(new UnityAction(MarkAllRead));
#endif

            MelonLogger.Msg("Read All button added to the Messages app.");
        }

        private static Font ResolveFont(MessagesApp app)
        {
            // Borrow a font from the game's own UI so the button looks native
            try
            {
                Font font = GetGameFont(app);
                if (font is not null)
                    return font;
            }
            catch
            {
                // fall through
            }

            try
            {
                return GetBuiltinFont();
            }
            catch
            {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Font GetGameFont(MessagesApp app)
        {
            Text appText = app.dialoguePageNameText;
            if (appText is not null && appText.font is not null)
                return appText.font;

            Text anyText = app.GetComponentInChildren<Text>(true);
            if (anyText is not null && anyText.font is not null)
                return anyText.font;

            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Font GetBuiltinFont()
        {
            // Built-in Unity font (name depends on the engine version)
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        private static void MarkAllRead()
        {
            var conversations = MessagesApp.Conversations;
            if (conversations is null)
            {
                MelonLogger.Warning("Conversation list does not exist yet.");
                return;
            }

            int marked = 0;
            int count = conversations.Count;
            for (int i = 0; i < count; i++)
            {
                MSGConversation conversation = conversations[i];
                if (conversation is null || conversation.Read)
                    continue;

                try
                {
                    conversation.SetRead(true);
                    marked++;
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Could not mark conversation '{conversation.contactName}' as read: {e.Message}");
                }
            }

            // Refresh the notification badge on the app icon
            try
            {
                MessagesApp app = GetMessagesApp();
                if (app is not null)
                    app.RefreshNotifications();
            }
            catch
            {
                // SetRead usually refreshes notifications by itself
            }

            MelonLogger.Msg($"Marked {marked} conversation(s) as read.");
        }
    }
}
