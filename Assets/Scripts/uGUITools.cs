using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

#if UNITY_EDITOR

using UnityEditor;

public class uGUITools : MonoBehaviour {
	[MenuItem("uGUI/Anchors to Corners %[")]
	static void AnchorsToCorners(){
		foreach(Transform transform in Selection.transforms){
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;

			if(t == null || pt == null) return;
			
			Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
			                                    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
			Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
			                                    t.anchorMax.y + t.offsetMax.y / pt.rect.height);

			t.anchorMin = newAnchorsMin;
			t.anchorMax = newAnchorsMax;
			t.offsetMin = t.offsetMax = new Vector2(0, 0);
		}
	}

	public static void AnchorsToCorners( RectTransform trans, RectTransform parent )
	{
		RectTransform t = trans;
		RectTransform pt = parent;

		if ( t == null || pt == null ) return;

		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
												t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
												t.anchorMax.y + t.offsetMax.y / pt.rect.height);

		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}

	[MenuItem("uGUI/Corners to Anchors %]")]
	static void CornersToAnchors()
	{
		foreach ( Transform transform in Selection.transforms )
		{
			RectTransform t = transform as RectTransform;

			if ( t == null ) return;

			t.offsetMin = t.offsetMax = new Vector2(0, 0);
		}
	}

	[MenuItem("uGUI/Mirror Horizontally Around Anchors %;")]
	static void MirrorHorizontallyAnchors()
	{
		MirrorHorizontally(false);
	}

	[MenuItem("uGUI/Mirror Horizontally Around Parent Center %:")]
	static void MirrorHorizontallyParent()
	{
		MirrorHorizontally(true);
	}

	static void MirrorHorizontally( bool mirrorAnchors )
	{
		foreach ( Transform transform in Selection.transforms )
		{
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;

			if ( t == null || pt == null ) return;

			if ( mirrorAnchors )
			{
				Vector2 oldAnchorMin = t.anchorMin;
				t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);
				t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);
			}

			Vector2 oldOffsetMin = t.offsetMin;
			t.offsetMin = new Vector2(-t.offsetMax.x, t.offsetMin.y);
			t.offsetMax = new Vector2(-oldOffsetMin.x, t.offsetMax.y);

			t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
		}
	}

	[MenuItem("uGUI/Mirror Vertically Around Anchors %'")]
	static void MirrorVerticallyAnchors()
	{
		MirrorVertically(false);
	}

	[MenuItem("uGUI/Mirror Vertically Around Parent Center %\"")]
	static void MirrorVerticallyParent()
	{
		MirrorVertically(true);
	}

	static void MirrorVertically( bool mirrorAnchors )
	{
		foreach ( Transform transform in Selection.transforms )
		{
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;

			if ( t == null || pt == null ) return;

			if ( mirrorAnchors )
			{
				Vector2 oldAnchorMin = t.anchorMin;
				t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);
				t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);
			}

			Vector2 oldOffsetMin = t.offsetMin;
			t.offsetMin = new Vector2(t.offsetMin.x, -t.offsetMax.y);
			t.offsetMax = new Vector2(t.offsetMax.x, -oldOffsetMin.y);

			t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);
		}
	}
}
#endif

public class UIDisableInput
{

    // Keep a list of pointer input modules that we have disabled so that we can re-enable them
    static List<PointerInputModule> disabled = new List<PointerInputModule>();

    static int disableCount = 0;    // How many times has disable been called?

    public static void Disable()
    {
        if (disableCount++ == 0)
        {
            UpdateState(false);
        }
    }

    public static void Enable(bool enable)
    {
        if (!enable)
        {
            Disable();
            return;
        }
        if (--disableCount == 0)
        {
            UpdateState(true);
            if (disableCount > 0)
            {
                Debug.LogWarning("Warning UIDisableInput.Enable called more than Disable");
            }
        }
    }

    static void UpdateState(bool enabled)
    {
        // First re-enable all systems
        for (int i = 0; i < disabled.Count; i++)
        {
            if (disabled[i])
            {
                disabled[i].enabled = true;
            }
        }
        disabled.Clear();

        EventSystem es = EventSystem.current;
        if (es == null) return;

        es.sendNavigationEvents = enabled;
        if (!enabled)
        {
            // Find all PointerInputModules and disable them
            PointerInputModule[] pointerInput = es.GetComponents<PointerInputModule>();
            if (pointerInput != null)
            {
                for (int i = 0; i < pointerInput.Length; i++)
                {
                    PointerInputModule pim = pointerInput[i];
                    if (pim.enabled)
                    {
                        pim.enabled = false;
                        // Keep a list of disabled ones
                        disabled.Add(pim);
                    }
                }
            }

            // Cause EventSystem to update it's list of modules
            es.enabled = false;
            es.enabled = true;
        }
    }
}