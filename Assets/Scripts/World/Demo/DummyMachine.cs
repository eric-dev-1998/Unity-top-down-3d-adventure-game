using UnityEngine;

public class DummyMachine : MonoBehaviour
{
    public GameObject dummyPrefab;
    private Animator anim;
    private GameObject currentDummy;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void SpawnDummy()
    {
        currentDummy = Instantiate(dummyPrefab, transform.Find("Placeholder"));
    }

    public void DestroyDummy()
    {
        Destroy(currentDummy);
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    public void Show()
    {
        anim.SetBool("Show", true);
    }
}
