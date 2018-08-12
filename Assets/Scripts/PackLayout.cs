using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Align horizontal, or vertical layout using a spcaing value that packs the children together.
[ExecuteInEditMode]
public class PackLayout : MonoBehaviour {
    private HorizontalLayoutGroup hlg;
    private VerticalLayoutGroup vlg;
    // Update is called when the scene is modified
    void Update() {
        hlg = GetComponent<HorizontalLayoutGroup>();
        vlg = GetComponent<VerticalLayoutGroup>();
        RectTransform containerRectTransform = GetComponent<RectTransform>();
        if (containerRectTransform != null) {
            if (hlg != null) {
                float childWidth = 0;
                foreach (Transform child in transform) {
                    if (child.gameObject.activeSelf) {
                        RectTransform rectTransform = child.GetComponent<RectTransform>();
                        if (rectTransform != null) {
                            childWidth += rectTransform.rect.width;
                        }
                    }
                }
                hlg.spacing = Mathf.Min((containerRectTransform.rect.width - childWidth) * -1, 0);
            }
            if (vlg != null) {
                float childHeight = 0;
                foreach (Transform child in transform) {
                    if (child.gameObject.activeSelf) {
                        RectTransform rectTransform = child.GetComponent<RectTransform>();
                        if (rectTransform != null) {
                            childHeight += rectTransform.rect.height;
                        }
                    }
                }
                vlg.spacing = Mathf.Min((containerRectTransform.rect.height - childHeight) * -1, 0);
            }
        }
        else {
            Debug.LogError("Object does not have a rectTransform, cannot pack children");
        }
    }
}