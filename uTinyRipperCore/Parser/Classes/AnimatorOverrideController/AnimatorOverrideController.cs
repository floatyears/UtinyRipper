﻿using System.Collections.Generic;
using uTinyRipper.AssetExporters;
using uTinyRipper.Classes.AnimatorOverrideControllers;
using uTinyRipper.Exporter.YAML;
using uTinyRipper.SerializedFiles;

namespace uTinyRipper.Classes
{
	public sealed class AnimatorOverrideController : RuntimeAnimatorController
	{
		public AnimatorOverrideController(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			Controller.Read(reader);
			m_clips = reader.ReadArray<AnimationClipOverride>();
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			
			yield return Controller.FetchDependency(file, isLog, ToLogString, "m_Controller");
			foreach (AnimationClipOverride clip in Clips)
			{
				foreach (Object @object in clip.FetchDependencies(file, isLog))
				{
					yield return @object;
				}
			}
		}

		public override bool IsContainsAnimationClip(AnimationClip clip)
		{
			foreach (AnimationClipOverride overClip in Clips)
			{
				if(overClip.OriginalClip.IsAsset(File, clip))
				{
					return true;
				}
				else if (overClip.OverrideClip.IsAsset(File, clip))
				{
					return true;
				}
			}
			RuntimeAnimatorController baseController = Controller.FindAsset(File);
			if(baseController != null)
			{
				return baseController.IsContainsAnimationClip(clip);
			}
			return false;
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add("m_Controller", Controller.ExportYAML(container));
			node.Add("m_Clips", Clips.ExportYAML(container));
			return node;
		}

		public override string ExportExtension => "overrideController";

		public IReadOnlyList<AnimationClipOverride> Clips => m_clips;

		public PPtr<RuntimeAnimatorController> Controller;

		private AnimationClipOverride[] m_clips;
	}
}
