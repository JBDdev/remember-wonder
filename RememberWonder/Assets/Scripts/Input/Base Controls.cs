//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Scripts/Input/Base Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @BaseControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @BaseControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Base Controls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""09f5b174-2826-4278-b2d6-cf8ba3b050cd"",
            ""actions"": [
                {
                    ""name"": ""MoveX"",
                    ""type"": ""Value"",
                    ""id"": ""0c6472ea-8f6c-4825-a53f-1793111e95de"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MoveY"",
                    ""type"": ""Value"",
                    ""id"": ""5a40168c-1be8-430d-8e96-a71772e8832a"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""1b7b029e-1c62-43bf-b28a-8ffd95bcf692"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LookX"",
                    ""type"": ""Value"",
                    ""id"": ""95fd8a73-dc9b-4465-be11-b354025386f3"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LookY"",
                    ""type"": ""Value"",
                    ""id"": ""50f4c6a5-0978-4a81-9a41-dd6fa2243e12"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""EnablePointerLook"",
                    ""type"": ""Button"",
                    ""id"": ""cd37d751-c032-4057-a18b-f351f19e7ed3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""588977eb-633d-4317-bce2-ebeb0a7b4b23"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Key Composite"",
                    ""id"": ""2fe3f8a8-9697-4ce8-9189-c2389921bf51"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveX"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d31e8ef1-a92a-415c-bfa3-3f7c9e0d13c9"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""11d74a54-a7e8-4561-8d8e-a2d73d04f4fc"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""505782fc-3ac7-4e0f-8dd1-fd50e419be5d"",
                    ""path"": ""<Gamepad>/leftStick/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Key Composite"",
                    ""id"": ""fcdd0bd1-d10a-4b85-aeec-7888516126ed"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveY"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""3518fa38-e8aa-483b-920c-57a7ea69efd8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""8a47ed90-ff11-4296-be05-568f9546a842"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b6d27360-4a3c-453a-a34b-8023ece2adcc"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""26849e76-f734-49b8-b31f-1c97868f6371"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82c9a73c-b286-4ee5-a64f-0d26ca00581e"",
                    ""path"": ""<Gamepad>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d3c32f44-a4b7-4f0c-89f7-a7860773f86a"",
                    ""path"": ""<Pointer>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53005fd0-bdf2-4499-aa2e-7082572195c7"",
                    ""path"": ""<Gamepad>/rightStick/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19088ca5-4b01-4a79-b482-49ee2b342385"",
                    ""path"": ""<Pointer>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3351e96c-8e44-4a8c-b357-104ed463f789"",
                    ""path"": ""<Pointer>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EnablePointerLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_MoveX = m_Gameplay.FindAction("MoveX", throwIfNotFound: true);
        m_Gameplay_MoveY = m_Gameplay.FindAction("MoveY", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
        m_Gameplay_LookX = m_Gameplay.FindAction("LookX", throwIfNotFound: true);
        m_Gameplay_LookY = m_Gameplay.FindAction("LookY", throwIfNotFound: true);
        m_Gameplay_EnablePointerLook = m_Gameplay.FindAction("EnablePointerLook", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_MoveX;
    private readonly InputAction m_Gameplay_MoveY;
    private readonly InputAction m_Gameplay_Jump;
    private readonly InputAction m_Gameplay_LookX;
    private readonly InputAction m_Gameplay_LookY;
    private readonly InputAction m_Gameplay_EnablePointerLook;
    public struct GameplayActions
    {
        private @BaseControls m_Wrapper;
        public GameplayActions(@BaseControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveX => m_Wrapper.m_Gameplay_MoveX;
        public InputAction @MoveY => m_Wrapper.m_Gameplay_MoveY;
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @LookX => m_Wrapper.m_Gameplay_LookX;
        public InputAction @LookY => m_Wrapper.m_Gameplay_LookY;
        public InputAction @EnablePointerLook => m_Wrapper.m_Gameplay_EnablePointerLook;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @MoveX.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveX;
                @MoveX.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveX;
                @MoveX.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveX;
                @MoveY.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveY;
                @MoveY.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveY;
                @MoveY.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveY;
                @Jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @LookX.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLookX;
                @LookX.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLookX;
                @LookX.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLookX;
                @LookY.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLookY;
                @LookY.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLookY;
                @LookY.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLookY;
                @EnablePointerLook.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEnablePointerLook;
                @EnablePointerLook.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEnablePointerLook;
                @EnablePointerLook.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEnablePointerLook;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveX.started += instance.OnMoveX;
                @MoveX.performed += instance.OnMoveX;
                @MoveX.canceled += instance.OnMoveX;
                @MoveY.started += instance.OnMoveY;
                @MoveY.performed += instance.OnMoveY;
                @MoveY.canceled += instance.OnMoveY;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @LookX.started += instance.OnLookX;
                @LookX.performed += instance.OnLookX;
                @LookX.canceled += instance.OnLookX;
                @LookY.started += instance.OnLookY;
                @LookY.performed += instance.OnLookY;
                @LookY.canceled += instance.OnLookY;
                @EnablePointerLook.started += instance.OnEnablePointerLook;
                @EnablePointerLook.performed += instance.OnEnablePointerLook;
                @EnablePointerLook.canceled += instance.OnEnablePointerLook;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMoveX(InputAction.CallbackContext context);
        void OnMoveY(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLookX(InputAction.CallbackContext context);
        void OnLookY(InputAction.CallbackContext context);
        void OnEnablePointerLook(InputAction.CallbackContext context);
    }
}
