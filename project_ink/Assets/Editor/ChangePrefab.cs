using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class MyEditorWindow : EditorWindow
{
    UnityEngine.Object target;
    VisualElement targetpptContainer;
    int duration=5;
    float currentTime=0;
    [MenuItem("Window/Replace")]
    public static void ShowExample()
    {
        MyEditorWindow wnd = GetWindow<MyEditorWindow>();
        wnd.titleContent = new GUIContent("Window");
    }

    void OnGUI(){
    }
    void CreateGUI(){
        // Root visual element
        var root = rootVisualElement;

        //object field
        ObjectField targetObjField=new ObjectField();
        targetObjField.objectType=typeof(MonoBehaviour);
        targetObjField.value=target;
        targetObjField.RegisterValueChangedCallback(OnTargetChanged);
        root.Add(targetObjField);


        //timeline container
        VisualElement container=new VisualElement();
        root.Add(container);
        //target object property container
        targetpptContainer=new VisualElement();
        targetpptContainer.style.flexBasis=Length.Percent(.3f);
        container.Add(targetpptContainer);

        // Create the timeline
        var timeline = new VisualElement();
        timeline.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        container.Add(timeline);

        // Draw timeline ticks
        for (float i = 0; i <= duration; i += 1f)
        {
            var tick = new VisualElement();
            tick.style.position = Position.Absolute;
            tick.style.left = (i / duration) * 100; // Percentage-based positioning
            tick.style.width = 1;
            tick.style.height = 10;
            tick.style.backgroundColor = Color.white;

            // Add a label for the tick
            var label = new Label(i.ToString());
            label.style.position = Position.Absolute;
            label.style.left = (i / duration) * 100 - 10; // Offset for centering
            label.style.top = 12;
            label.style.color = Color.white;

            timeline.Add(tick);
            timeline.Add(label);
        }

        // Current time indicator
        var timeIndicator = new VisualElement();
        timeIndicator.style.position = Position.Absolute;
        timeIndicator.style.width = 2;
        timeIndicator.style.height = 30;
        timeIndicator.style.backgroundColor = Color.red;
        timeline.Add(timeIndicator);

        // Slider to control the current time
        var timeSlider = new Slider("Time", 0, duration);
        timeSlider.RegisterValueChangedCallback(evt =>
        {
            currentTime = evt.newValue;
            timeIndicator.style.left = (currentTime / duration) * 100; // Update indicator position
        });
        root.Add(timeSlider);
    }
    void OnTargetChanged(ChangeEvent<UnityEngine.Object> evt){
        target=evt.newValue;
        targetpptContainer.Clear();
        if(target==null)
            return;
        // Use reflection to get the object's properties and fields
        SerializedObject sdTarget=new SerializedObject(target);
        SerializedProperty it=sdTarget.GetIterator();
        
        //skip useless fields
        it.Next(true);
        for(int i=0;i<9;++i)
            it.Next(false);
        //create propertyfield
        while(it.Next(false)){
            PropertyField field=new PropertyField(it);
            field.Bind(sdTarget);
            targetpptContainer.Add(field);
        }
        return;
    }
}