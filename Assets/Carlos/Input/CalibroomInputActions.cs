// GENERATED AUTOMATICALLY FROM 'Assets/Carlos/Input/CalibroomInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @CalibroomInputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @CalibroomInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CalibroomInputActions"",
    ""maps"": [
        {
            ""name"": ""Menu"",
            ""id"": ""9abc1b2f-3667-4d2e-bd33-850370ee2d6a"",
            ""actions"": [
                {
                    ""name"": ""ExitGame"",
                    ""type"": ""Button"",
                    ""id"": ""fe83f3ba-d781-462d-a97d-320340f9c858"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CompletePuzzle"",
                    ""type"": ""Button"",
                    ""id"": ""6d309663-7c53-4855-8038-afaeea7b1f4b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleDataCollection"",
                    ""type"": ""Button"",
                    ""id"": ""cfd13600-9f06-41d3-a92e-5b0e468096b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3a3f0b1e-c770-4a8d-b08d-a88685a21231"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ExitGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bfb7bca1-3573-406f-9baf-ad66fd946f44"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CompletePuzzle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""74a78dc5-7d42-4fa4-8ada-197a2d5e78aa"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleDataCollection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_ExitGame = m_Menu.FindAction("ExitGame", throwIfNotFound: true);
        m_Menu_CompletePuzzle = m_Menu.FindAction("CompletePuzzle", throwIfNotFound: true);
        m_Menu_ToggleDataCollection = m_Menu.FindAction("ToggleDataCollection", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_ExitGame;
    private readonly InputAction m_Menu_CompletePuzzle;
    private readonly InputAction m_Menu_ToggleDataCollection;
    public struct MenuActions
    {
        private @CalibroomInputActions m_Wrapper;
        public MenuActions(@CalibroomInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ExitGame => m_Wrapper.m_Menu_ExitGame;
        public InputAction @CompletePuzzle => m_Wrapper.m_Menu_CompletePuzzle;
        public InputAction @ToggleDataCollection => m_Wrapper.m_Menu_ToggleDataCollection;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @ExitGame.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnExitGame;
                @ExitGame.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnExitGame;
                @ExitGame.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnExitGame;
                @CompletePuzzle.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnCompletePuzzle;
                @CompletePuzzle.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnCompletePuzzle;
                @CompletePuzzle.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnCompletePuzzle;
                @ToggleDataCollection.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnToggleDataCollection;
                @ToggleDataCollection.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnToggleDataCollection;
                @ToggleDataCollection.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnToggleDataCollection;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ExitGame.started += instance.OnExitGame;
                @ExitGame.performed += instance.OnExitGame;
                @ExitGame.canceled += instance.OnExitGame;
                @CompletePuzzle.started += instance.OnCompletePuzzle;
                @CompletePuzzle.performed += instance.OnCompletePuzzle;
                @CompletePuzzle.canceled += instance.OnCompletePuzzle;
                @ToggleDataCollection.started += instance.OnToggleDataCollection;
                @ToggleDataCollection.performed += instance.OnToggleDataCollection;
                @ToggleDataCollection.canceled += instance.OnToggleDataCollection;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    public interface IMenuActions
    {
        void OnExitGame(InputAction.CallbackContext context);
        void OnCompletePuzzle(InputAction.CallbackContext context);
        void OnToggleDataCollection(InputAction.CallbackContext context);
    }
}
