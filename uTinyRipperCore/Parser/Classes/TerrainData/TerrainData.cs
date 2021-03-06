﻿using System.Collections.Generic;
using uTinyRipper.AssetExporters;
using uTinyRipper.Classes.TerrainDatas;
using uTinyRipper.Exporter.YAML;
using uTinyRipper.SerializedFiles;

namespace uTinyRipper.Classes
{
	public sealed class TerrainData : NamedObject
	{
		public TerrainData(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		/// <summary>
		/// Less than 3.0.0
		/// </summary>
		public static bool IsReadLightmap(Version version)
		{
			return version.IsLess(3);
		}

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			SplatDatabase.Read(reader);
			DetailDatabase.Read(reader);
			Heightmap.Read(reader);
			if (IsReadLightmap(reader.Version))
			{
				Lightmap.Read(reader);
			}
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			
			foreach(Object @object in SplatDatabase.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			foreach(Object @object in DetailDatabase.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			foreach(Object @object in Heightmap.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			
			if (IsReadLightmap(file.Version))
			{
				yield return Lightmap.FetchDependency(file, isLog, ToLogString, "m_Lightmap");
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add("m_SplatDatabase", SplatDatabase.ExportYAML(container));
			node.Add("m_DetailDatabase", DetailDatabase.ExportYAML(container));
			node.Add("m_Heightmap", Heightmap.ExportYAML(container));
			return node;
		}

		public SplatDatabase SplatDatabase;
		public DetailDatabase DetailDatabase;
		public Heightmap Heightmap;
		public PPtr<Texture2D> Lightmap;
	}
}
