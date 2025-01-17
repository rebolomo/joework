﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_RendererWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(UnityEngine.Renderer), typeof(UnityEngine.Component));
		L.RegFunction("SetPropertyBlock", SetPropertyBlock);
		L.RegFunction("GetPropertyBlock", GetPropertyBlock);
		L.RegFunction("GetClosestReflectionProbes", GetClosestReflectionProbes);
		L.RegFunction("New", _CreateUnityEngine_Renderer);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", Lua_ToString);
		L.RegVar("isPartOfStaticBatch", get_isPartOfStaticBatch, null);
		L.RegVar("worldToLocalMatrix", get_worldToLocalMatrix, null);
		L.RegVar("localToWorldMatrix", get_localToWorldMatrix, null);
		L.RegVar("enabled", get_enabled, set_enabled);
		L.RegVar("shadowCastingMode", get_shadowCastingMode, set_shadowCastingMode);
		L.RegVar("receiveShadows", get_receiveShadows, set_receiveShadows);
		L.RegVar("material", get_material, set_material);
		L.RegVar("sharedMaterial", get_sharedMaterial, set_sharedMaterial);
		L.RegVar("materials", get_materials, set_materials);
		L.RegVar("sharedMaterials", get_sharedMaterials, set_sharedMaterials);
		L.RegVar("bounds", get_bounds, null);
		L.RegVar("lightmapIndex", get_lightmapIndex, set_lightmapIndex);
		L.RegVar("realtimeLightmapIndex", get_realtimeLightmapIndex, null);
		L.RegVar("lightmapScaleOffset", get_lightmapScaleOffset, set_lightmapScaleOffset);
		L.RegVar("realtimeLightmapScaleOffset", get_realtimeLightmapScaleOffset, set_realtimeLightmapScaleOffset);
		L.RegVar("isVisible", get_isVisible, null);
		L.RegVar("useLightProbes", get_useLightProbes, set_useLightProbes);
		L.RegVar("probeAnchor", get_probeAnchor, set_probeAnchor);
		L.RegVar("reflectionProbeUsage", get_reflectionProbeUsage, set_reflectionProbeUsage);
		L.RegVar("sortingLayerName", get_sortingLayerName, set_sortingLayerName);
		L.RegVar("sortingLayerID", get_sortingLayerID, set_sortingLayerID);
		L.RegVar("sortingOrder", get_sortingOrder, set_sortingOrder);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateUnityEngine_Renderer(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				UnityEngine.Renderer obj = new UnityEngine.Renderer();
				ToLua.Push(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: UnityEngine.Renderer.New");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetPropertyBlock(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)ToLua.CheckObject(L, 1, typeof(UnityEngine.Renderer));
			UnityEngine.MaterialPropertyBlock arg0 = (UnityEngine.MaterialPropertyBlock)ToLua.CheckObject(L, 2, typeof(UnityEngine.MaterialPropertyBlock));
			obj.SetPropertyBlock(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetPropertyBlock(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)ToLua.CheckObject(L, 1, typeof(UnityEngine.Renderer));
			UnityEngine.MaterialPropertyBlock arg0 = (UnityEngine.MaterialPropertyBlock)ToLua.CheckObject(L, 2, typeof(UnityEngine.MaterialPropertyBlock));
			obj.GetPropertyBlock(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClosestReflectionProbes(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)ToLua.CheckObject(L, 1, typeof(UnityEngine.Renderer));
			System.Collections.Generic.List<UnityEngine.Rendering.ReflectionProbeBlendInfo> arg0 = (System.Collections.Generic.List<UnityEngine.Rendering.ReflectionProbeBlendInfo>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.Rendering.ReflectionProbeBlendInfo>));
			obj.GetClosestReflectionProbes(arg0);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Lua_ToString(IntPtr L)
	{
		object obj = ToLua.ToObject(L, 1);

		if (obj != null)
		{
			LuaDLL.lua_pushstring(L, obj.ToString());
		}
		else
		{
			LuaDLL.lua_pushnil(L);
		}

		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isPartOfStaticBatch(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			bool ret = obj.isPartOfStaticBatch;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index isPartOfStaticBatch on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_worldToLocalMatrix(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Matrix4x4 ret = obj.worldToLocalMatrix;
			ToLua.PushValue(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index worldToLocalMatrix on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_localToWorldMatrix(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Matrix4x4 ret = obj.localToWorldMatrix;
			ToLua.PushValue(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index localToWorldMatrix on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_enabled(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			bool ret = obj.enabled;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index enabled on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_shadowCastingMode(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Rendering.ShadowCastingMode ret = obj.shadowCastingMode;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index shadowCastingMode on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_receiveShadows(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			bool ret = obj.receiveShadows;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index receiveShadows on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_material(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Material ret = obj.material;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index material on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_sharedMaterial(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Material ret = obj.sharedMaterial;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sharedMaterial on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_materials(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Material[] ret = obj.materials;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index materials on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_sharedMaterials(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Material[] ret = obj.sharedMaterials;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sharedMaterials on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_bounds(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Bounds ret = obj.bounds;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index bounds on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_lightmapIndex(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			int ret = obj.lightmapIndex;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index lightmapIndex on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_realtimeLightmapIndex(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			int ret = obj.realtimeLightmapIndex;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index realtimeLightmapIndex on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_lightmapScaleOffset(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Vector4 ret = obj.lightmapScaleOffset;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index lightmapScaleOffset on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_realtimeLightmapScaleOffset(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Vector4 ret = obj.realtimeLightmapScaleOffset;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index realtimeLightmapScaleOffset on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isVisible(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			bool ret = obj.isVisible;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index isVisible on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_useLightProbes(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			bool ret = obj.useLightProbes;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index useLightProbes on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_probeAnchor(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Transform ret = obj.probeAnchor;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index probeAnchor on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_reflectionProbeUsage(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Rendering.ReflectionProbeUsage ret = obj.reflectionProbeUsage;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index reflectionProbeUsage on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_sortingLayerName(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			string ret = obj.sortingLayerName;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sortingLayerName on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_sortingLayerID(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			int ret = obj.sortingLayerID;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sortingLayerID on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_sortingOrder(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			int ret = obj.sortingOrder;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sortingOrder on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_enabled(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.enabled = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index enabled on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_shadowCastingMode(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Rendering.ShadowCastingMode arg0 = (UnityEngine.Rendering.ShadowCastingMode)ToLua.CheckObject(L, 2, typeof(UnityEngine.Rendering.ShadowCastingMode));
			obj.shadowCastingMode = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index shadowCastingMode on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_receiveShadows(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.receiveShadows = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index receiveShadows on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_material(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Material arg0 = (UnityEngine.Material)ToLua.CheckUnityObject(L, 2, typeof(UnityEngine.Material));
			obj.material = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index material on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_sharedMaterial(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Material arg0 = (UnityEngine.Material)ToLua.CheckUnityObject(L, 2, typeof(UnityEngine.Material));
			obj.sharedMaterial = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sharedMaterial on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_materials(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Material[] arg0 = ToLua.CheckObjectArray<UnityEngine.Material>(L, 2);
			obj.materials = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index materials on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_sharedMaterials(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Material[] arg0 = ToLua.CheckObjectArray<UnityEngine.Material>(L, 2);
			obj.sharedMaterials = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sharedMaterials on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_lightmapIndex(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.lightmapIndex = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index lightmapIndex on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_lightmapScaleOffset(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Vector4 arg0 = ToLua.ToVector4(L, 2);
			obj.lightmapScaleOffset = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index lightmapScaleOffset on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_realtimeLightmapScaleOffset(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Vector4 arg0 = ToLua.ToVector4(L, 2);
			obj.realtimeLightmapScaleOffset = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index realtimeLightmapScaleOffset on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_useLightProbes(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.useLightProbes = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index useLightProbes on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_probeAnchor(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Transform arg0 = (UnityEngine.Transform)ToLua.CheckUnityObject(L, 2, typeof(UnityEngine.Transform));
			obj.probeAnchor = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index probeAnchor on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_reflectionProbeUsage(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			UnityEngine.Rendering.ReflectionProbeUsage arg0 = (UnityEngine.Rendering.ReflectionProbeUsage)ToLua.CheckObject(L, 2, typeof(UnityEngine.Rendering.ReflectionProbeUsage));
			obj.reflectionProbeUsage = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index reflectionProbeUsage on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_sortingLayerName(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.sortingLayerName = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sortingLayerName on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_sortingLayerID(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.sortingLayerID = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sortingLayerID on a nil value" : e.Message);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_sortingOrder(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Renderer obj = (UnityEngine.Renderer)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.sortingOrder = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o == null ? "attempt to index sortingOrder on a nil value" : e.Message);
		}
	}
}

