using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;
using Assets.Scripts.GameText;

namespace Assets.Scripts.Dialogue_System
{
    public class Manager : MonoBehaviour
    {
        public TextManager textManager;

        private UIDocument document;
        private VisualElement dialogueFrame;
        private VisualElement dialogueAnswers;
        private VisualElement progressIcon;
        private Label dialogueAuthor;
        private Label dialogueText;
        private UnityEngine.UIElements.Button buttonA;
        private UnityEngine.UIElements.Button buttonB;

        // Alignment properties.
        public bool center;
        public bool show;
        public bool chooseA = false;
        public bool chooseB = false;
        public bool question = false;

        public float timeBetweenEachCharacter = 0.05f;

        private Animator itemDisplayAnimator;
        private GameObject itemDisplay;

        // Dialogue frame object and textures.
        private GameObject frame;
        private Sprite frame_center;
        private Sprite frame_bottom;
        private UnityEngine.UI.Image dialogue_icon;

        // Dialogue audio properties:
        private AudioSource audio_voice;
        private AudioSource audio_start;
        private AudioSource audio_end;

        // Text objects.
        private TextMeshProUGUI text_bottom;
        private TextMeshProUGUI text_center;
        private TextMeshProUGUI text_container;
        private TextMeshProUGUI text_author;

        // Continue and End labels to show at the end of each dialogue line.
        private GameObject button_continue;
        private GameObject button_end;

        // Dialogue question objects.
        private GameObject answerContainer;
        private UnityEngine.UI.Button answerA;
        private UnityEngine.UI.Button answerB;
        private bool isAsking = false;

        // References the player object.
        MainPlayer player;

        // Dialogue system properties:
        Dialogue currentDialogue;
        bool onDialogue = false;
        bool isWriting = false;
        int lineCounter = 0;
        string currentLine = "";

        // New system properties:
        public bool advance = false;
        bool advancePressedOnce = false;
        bool finishedWriting = false;

        // Current coroutine instance reference:
        IEnumerator textWritingCoroutine = null;

        // Animator component to play dialogue box animations.
        Animator anim;

        private void Start()
        {
            textManager = FindAnyObjectByType<TextManager>();
            if (textManager == null)
                Debug.LogError("[Text manager]: No text manager was found on scene.");

            document = GetComponent<UIDocument>();

            dialogueFrame = document.rootVisualElement.Q<VisualElement>("Frame");
            dialogueAnswers = document.rootVisualElement.Q<VisualElement>("Answers");
            progressIcon = document.rootVisualElement.Q<VisualElement>("ProgressIcon");
            dialogueAuthor = document.rootVisualElement.Q<Label>("Author");
            dialogueText = document.rootVisualElement.Q<Label>("Text");
            buttonA = document.rootVisualElement.Q<UnityEngine.UIElements.Button>("ButtonA");
            buttonA.clicked += PickA;

            buttonB = document.rootVisualElement.Q<UnityEngine.UIElements.Button>("ButtonB");
            buttonB.clicked += PickB;

            player = FindAnyObjectByType<MainPlayer>();

            LoadAudio();
        }

        private void Update()
        {
            if (onDialogue)
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (isWriting)
                    {
                        advancePressedOnce = true;
                    }
                    else
                    {
                        if (!question)
                            advance = true;
                    }
                }
        }

        public IEnumerator ShowDialogueBox()
        {
            dialogueAuthor.text = "";
            dialogueText.text = "";
            dialogueFrame.RemoveFromClassList("frame-hidden");

            player.LockMovement();

            audio_start.Play();

            chooseA = false;
            chooseB = false;
            onDialogue = true;
            yield return new WaitForSeconds(0.35f);
        }

        public IEnumerator HideDialogueBox()
        {
            dialogueAuthor.text = "";
            dialogueText.text = "";
            dialogueFrame.AddToClassList("frame-hidden");

            player.UnlockMovement();

            audio_end.Play();

            yield return new WaitForSeconds(0.35f);
            onDialogue = false;
            advance = false;
            advancePressedOnce = false;
            finishedWriting = false;
        }

        public void HideAnswers()
        {
            dialogueAnswers.AddToClassList("answers-hidden");
        }

        public void PickA()
        {
            chooseA = true;
            advance = true;
        }

        public void PickB()
        {
            chooseB = true;
            advance = true;
        }

        public bool OnDialogue() { return onDialogue; }

        public void Load()
        {
            // Load main player data.
            player = GameObject.FindAnyObjectByType<MainPlayer>();

            // Get frame object and sprites.
            frame = GameObject.Find("Dialogue_Frame");
            frame_bottom =  AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Dialogue System/Frame_Bottom.psd");
            frame_center = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Dialogue System/Frame_Center.psd");

            dialogue_icon = GameObject.Find("Dialogue_Icon").GetComponent<UnityEngine.UI.Image>();
            dialogue_icon.gameObject.SetActive(false);

            // Load text objects.
            text_author = GameObject.Find("Dialogue_Author").GetComponent<TextMeshProUGUI>();
            text_bottom = GameObject.Find("Dialogue_Bottom").GetComponent<TextMeshProUGUI>();
            text_center = GameObject.Find("Dialogue_Center").GetComponent<TextMeshProUGUI>();

            // Load labels.
            button_continue = GameObject.Find("Dialogue_ContinueButton");
            button_end = GameObject.Find("Dialogue_EndButton");

            // Load answer objects.
            answerContainer = GameObject.Find("Dialogue_Answer");

            answerA = GameObject.Find("Dialogue_Answer_A").GetComponent<UnityEngine.UI.Button>();
            answerA.onClick.AddListener(PickA);

            answerB = GameObject.Find("Dialogue_Answer_B").GetComponent<UnityEngine.UI.Button>();
            answerB.onClick.AddListener(PickB);

            // Item display object parent for item dialogues.
            itemDisplay = Camera.main.transform.Find("Item display").gameObject;

            // Load animator.
            anim = GetComponent<Animator>();
            if (!anim)
            {
                Debug.LogError("Couldn't load [Dialogue animator]. The dialogue system cannot be started.");
                return;
            }

            // Init dialogue system.
            Init();
        }

        private void LoadAudio()
        {
            audio_voice = GameObject.Find("Audio/UI/Dialogue_voice").GetComponent<AudioSource>();
            audio_start = GameObject.Find("Audio/UI/Dialogue_start").GetComponent<AudioSource>();
            audio_end = GameObject.Find("Audio/UI/Dialogue_end").GetComponent<AudioSource>();
        }

        private void Init()
        {
            // Show dialogue box:

            anim.SetBool("Show", false);
            answerContainer.SetActive(false);
            button_continue.SetActive(false);
            button_end.SetActive(false);
        }

        public void AlignBottom()
        {
            dialogueFrame.RemoveFromClassList("frame-center");

            RectTransform box = frame.GetComponent<RectTransform>();

            box.position = new Vector3(box.rect.width / 2, 0, 0);
            box.GetComponent<UnityEngine.UI.Image>().sprite = frame_bottom;
            text_container = text_bottom;
            text_container.text = "";
            text_author.text = "";
            text_bottom.gameObject.SetActive(true);
            text_center.gameObject.SetActive(false);

            center = false;
        }

        public void AlignCenter()
        {
            dialogueFrame.AddToClassList("frame-center");

            RectTransform box = frame.GetComponent<RectTransform>();

            box.position = new Vector3(box.rect.width / 2, (Screen.height / 2) - (box.rect.height / 2), 0);
            box.GetComponent<UnityEngine.UI.Image>().sprite = frame_center;
            text_container = text_center;
            text_container.text = "";
            text_author.text = "";
            text_bottom.gameObject.SetActive(false);
            text_center.gameObject.SetActive(true);

            center = true;
        }

        public IEnumerator DisplayItem(string item_id)
        {
            // Search item in database.
            // Get item display asset and instantiate it.
            // Put item display inside the display parent with an animation.

            ItemText itemText = textManager.GetItem(item_id);
            if (itemText == null)
                yield break;

            string title = textManager.GetUIText("item_obtained") + itemText.name;
            string content = itemText.description;

            itemDisplayAnimator = Camera.main.transform.Find("Item display").GetComponent<Animator>();
            Inventory_System.InventoryManager inventoryManager = FindAnyObjectByType<Inventory_System.InventoryManager>();

            Inventory_System.Item item = inventoryManager.itemDatabase.GetItemById(item_id);
            if (item == null)
            {
                Debug.LogError($"[Dialogue manager]: DisplayItem() -> No item with id '{item_id}' could be found.");
                yield break;
            }

            Instantiate(item.item_display, Camera.main.transform.Find("Item display/Container").transform);
            itemDisplayAnimator.SetBool("Show", true);

            yield return new WaitForSeconds(0.25F);
            yield return StartCoroutine(WriteText(title, content, false, null, null));
        }

        public void EndItemDisplay()
        {
            // Hide item display with animation.
            // Destroy item display at animation end.

            itemDisplayAnimator = Camera.main.transform.Find("Item display").GetComponent<Animator>();
            itemDisplayAnimator.SetBool("Show", false);
        }

        public IEnumerator WriteText(string author, string line, bool isQuestion, string a, string b)
        {
            chooseA = false;
            chooseB = false;

            // Play voice aduio:
            audio_voice.Play();

            question = isQuestion;
            advance = false;
            advancePressedOnce = false;

            // Clear dialogue text to start writing a new line.

            dialogueText.text = "";
            dialogueAuthor.text = author;

            currentLine = "";
            progressIcon.style.visibility = Visibility.Hidden;

            // Set "isWriting" to true so the dialogue system knows a line is currently beign written.
            isWriting = true;

            foreach (char letter in line)
            {
                if (advancePressedOnce)
                {
                    dialogueText.text = line;
                    break;
                }
                else
                {

                    // Write current dialogue line letter by letter.
                    currentLine += letter;

                    dialogueText.text = currentLine;

                    yield return new WaitForSeconds(timeBetweenEachCharacter);
                }
            }

            finishedWriting = true;

            // Stop voice audio:
            audio_voice.Stop();

            if (question)
            {
                // Set buttons text:
                buttonA.text = a;
                buttonB.text = b;

                // Show answers:
                dialogueAnswers.RemoveFromClassList("answers-hidden");
            }
            else
            {
                // Check if this is the last line to display the correct dialogue box button.
                progressIcon.style.visibility = Visibility.Visible;
            }

            // Reset writting state to false.
            isWriting = false;
        }
    }
}
