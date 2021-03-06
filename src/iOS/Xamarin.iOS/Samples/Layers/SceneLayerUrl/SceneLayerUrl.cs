// Copyright 2017 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using System;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using UIKit;

namespace ArcGISRuntime.Samples.SceneLayerUrl
{
    [Register("SceneLayerUrl")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "ArcGIS scene layer (URL)",
        "Layers",
        "Display an ArcGIS Scene layer from a service.",
        "")]
    public class SceneLayerUrl : UIViewController
    {
        // Hold a reference to the SceneView.
        private SceneView _mySceneView;

        // URL for a service to use as an elevation source.
        private readonly Uri _elevationSourceUrl = new Uri(
            "https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer");

        // URL for the scene layer.
        private readonly Uri _serviceUri = new Uri(
            "https://scenesampleserverdev.arcgis.com/arcgis/rest/services/Hosted/Buildings_Philadelphia/SceneServer");

        public SceneLayerUrl()
        {
            Title = "ArcGIS scene layer (URL)";
        }

        public override void LoadView()
        {
            _mySceneView = new SceneView();
            _mySceneView.TranslatesAutoresizingMaskIntoConstraints = false;

            View = new UIView();
            View.AddSubviews(_mySceneView);

            _mySceneView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            _mySceneView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            _mySceneView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
            _mySceneView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Initialize();
        }

        private async void Initialize()
        {
            // Create new Scene.
            Scene myScene = new Scene {Basemap = Basemap.CreateImagery()};

            // Create and add an elevation source for the Scene.
            ArcGISTiledElevationSource elevationSrc = new ArcGISTiledElevationSource(_elevationSourceUrl);
            myScene.BaseSurface.ElevationSources.Add(elevationSrc);

            // Create new scene layer from the URL.
            ArcGISSceneLayer sceneLayer = new ArcGISSceneLayer(_serviceUri);

            // Add created layer to the operational layers collection.
            myScene.OperationalLayers.Add(sceneLayer);

            // Load the layer.
            await sceneLayer.LoadAsync();

            // Get the center of the scene layer.
            MapPoint center = (MapPoint)GeometryEngine.Project(sceneLayer.FullExtent.GetCenter(), SpatialReferences.Wgs84);

            // Create a camera with coordinates showing layer data.
            Camera camera = new Camera(center.Y, center.X, 225, 240, 80, 0);

            // Assign the Scene to the SceneView.
            _mySceneView.Scene = myScene;

            // Set view point of scene view using camera.
            await _mySceneView.SetViewpointCameraAsync(camera);
        }
    }
}