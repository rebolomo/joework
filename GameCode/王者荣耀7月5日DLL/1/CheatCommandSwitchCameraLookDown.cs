using System;

[CheatCommand("Fow/SwitchCameraLookDown", "俯视镜头", 0)]
internal class CheatCommandSwitchCameraLookDown : CheatCommandCommon
{
	private float m_oldRotX;

	private float m_oldMinZoom;

	private float m_oldMaxZoom;

	private float m_oldDefaultZoom;

	private float m_oldCurZoom;

	public static bool ms_bLookDown
	{
		get;
		private set;
	}

	private void RecordCameraSettings()
	{
		this.m_oldRotX = MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.rotation.defualtRotation.x;
		this.m_oldMinZoom = MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.minZoom;
		this.m_oldMaxZoom = MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.maxZoom;
		this.m_oldDefaultZoom = MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.defaultZoom;
		this.m_oldCurZoom = MonoSingleton<CameraSystem>.get_instance().MobaCamera.currentZoomAmount;
	}

	private void RecoverCameraSettings()
	{
		MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.rotation.defualtRotation.x = this.m_oldRotX;
		MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.minZoom = this.m_oldMinZoom;
		MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.maxZoom = this.m_oldMaxZoom;
		MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.defaultZoom = this.m_oldDefaultZoom;
	}

	protected override string Execute(string[] InArguments)
	{
		if (MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.lockTarget)
		{
			if (!CheatCommandSwitchCameraLookDown.ms_bLookDown)
			{
				CheatCommandSwitchCameraLookDown.ms_bLookDown = true;
				this.RecordCameraSettings();
				MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.rotation.defualtRotation.x = 90f;
				MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.minZoom = 20f;
				MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.maxZoom = 80f;
				MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.defaultZoom = 35f;
				MonoSingleton<CameraSystem>.get_instance().SetFocusActorForce(MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.lockTarget, MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.zoom.defaultZoom);
			}
			else
			{
				CheatCommandSwitchCameraLookDown.ms_bLookDown = false;
				this.RecoverCameraSettings();
				MonoSingleton<CameraSystem>.get_instance().SetFocusActorForce(MonoSingleton<CameraSystem>.get_instance().MobaCamera.settings.lockTarget, this.m_oldCurZoom);
			}
		}
		return CheatCommandBase.Done;
	}
}
