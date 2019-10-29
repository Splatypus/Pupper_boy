using UnityEditor;

using UnityEngine;
using UnityEngine.Events;

public class ConfirmationPopup : EditorWindow
{
   public string customMessage;
   public UnityEvent OnConfirm;
   public UnityEvent OnCancel;
   const int defaultWidth = 444;
   static Vector2 defaultSize = new Vector2(defaultWidth, (defaultWidth / 2.5f));

   public static ConfirmationPopup ShowConfirmationPopup(string customMessage = "Are You Sure?")
   {
      var window = GetWindow<ConfirmationPopup>(true, "Are You Sure?", true);

      window.OnConfirm = new UnityEvent();
      window.OnCancel = new UnityEvent();
      window.customMessage = customMessage;

      var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
      mousePos -= (defaultSize / 2);
      mousePos += new Vector2(0, 1) * 25;

      window.position = new Rect(mousePos, defaultSize);
      window.Show();

      return window;
   }

   private void OnGUI()
   {
      GUIStyle myLabel = new GUIStyle(GUI.skin.label)
      {
         fontSize = 15,
         wordWrap = true,
         fontStyle = FontStyle.Bold
      };

      GUIStyle myButton = new GUIStyle(GUI.skin.button)
      {
         fontSize = 14
      };

      GUILayout.Space(5);

      GUILayout.BeginVertical(); //-----

      GUILayout.Label(customMessage, myLabel);

      GUILayout.FlexibleSpace(); //-----//

      GUILayout.BeginHorizontal(); //-----

      if (GUILayout.Button("Yes", myButton))
      {
         OnConfirm.Invoke();
         Close();
      }

      GUILayout.Space(33);

      if (GUILayout.Button("No", myButton))
      {
         OnCancel.Invoke();
         Close();
      }

      GUILayout.EndHorizontal(); ///-----

      GUILayout.Space(15);

      GUILayout.EndVertical(); ///-----
   }
}
