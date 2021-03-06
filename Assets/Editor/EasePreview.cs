using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class EasePreview : EditorWindow
{
	private float _overshootOrAmplitude;

	private float _period;

	private Ease _easeType;

	private List<Vector2> _points = new List<Vector2>();

	private VisualElement _drawPanel;

	private FloatField _overField;

	private FloatField _periodField;

	private bool _first = true;

	[MenuItem( "Dse/EasePreview" )]
	public static void ShowExample()
	{
		EasePreview wnd = GetWindow<EasePreview>();
		wnd.titleContent = new GUIContent( "EasePreview" );
		wnd.Show();
		wnd.minSize = new Vector2( 400, 600 );
		wnd.maxSize = new Vector2( 400, 600 );
	}

	public void CreateGUI()
	{
		VisualElement root = rootVisualElement;

		var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( "Assets/Editor/EasePreview.uxml" );
		VisualElement labelFromUXML = visualTree.Instantiate();
		labelFromUXML.style.flexGrow = 1;
		root.Add( labelFromUXML );
		var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>( "Assets/Editor/EasePreview.uss" );
		root.styleSheets.Add( styleSheet );

		var selector = root.Q<EnumField>( "selector" );
		selector.Init( _easeType );
		selector.RegisterValueChangedCallback( SelectorValueChanged );

		_drawPanel = root.Q<VisualElement>( "drawPanel" );
		_drawPanel.generateVisualContent = DrawEaseFunction;

		_overField = root.Q<FloatField>( "overshootOrAmplitude" );
		_overField.RegisterValueChangedCallback( ParaChangedOvershootOrAmplitude );
		_overField.visible = false;
		_periodField = root.Q<FloatField>( "period" );
		_periodField.RegisterValueChangedCallback( ParaChangedPeriod );
		_periodField.visible = false;

		root.Bind( new SerializedObject( this ) );
	}



	private void ParaChangedPeriod( ChangeEvent<float> evt )
	{
		_period = evt.newValue;
		CaculationPoints();
		_drawPanel.MarkDirtyRepaint();
	}

	private void ParaChangedOvershootOrAmplitude( ChangeEvent<float> evt )
	{
		_overshootOrAmplitude = evt.newValue;
		CaculationPoints();
		_drawPanel.MarkDirtyRepaint();
	}

	private void SelectorValueChanged( ChangeEvent<System.Enum> evt )
	{
		_easeType = (Ease)evt.newValue;
		CaculationPoints();
		switch( _easeType )
		{
			case Ease.InElastic:
			case Ease.OutElastic:
			case Ease.InOutElastic:
			case Ease.Flash:
			case Ease.InFlash:
			case Ease.InOutFlash:
			case Ease.OutFlash:
			case Ease.InBounce:
			case Ease.InOutBounce:
			case Ease.OutBounce:
				_periodField.visible = true;
				_overField.visible = true;
				break;
			default:
				_periodField.visible = false;
				_overField.visible = false;
				break;
		}
		_drawPanel.MarkDirtyRepaint();
	}

	private void CaculationPoints()
	{
		var world_W = _drawPanel.worldBound.width - 50;
		var world_H = _drawPanel.worldBound.height - 50;
		_points.Clear();
		for( int i = 1; i < 50; i++ )
		{
			var y = ( 1 - EaseManager.Evaluate( _easeType, null, i * 0.02f, 1, _overshootOrAmplitude, _period ) ) * world_H + 25;
			var x = i * 0.02f * world_W + 25;
			if( float.IsNaN( x ) )
			{
				x = 1000;
			}
			if( float.IsNaN( y ) )
			{
				y = 1000;
			}
			_points.Add( new Vector2( x, y ) );
		}
	}

	private Vector2 NormalVector( Vector2 dir )
	{
		var a = dir.x;
		var b = dir.y;
		var x = -b * Mathf.Sqrt( 1 / ( b * b + a * a ) );
		var y = Mathf.Sqrt( a * a / ( a * a + b * b ) );
		return new Vector2( x, y );
	}

	private void DrawEaseFunction( MeshGenerationContext context )
	{
		if( _first )
		{
			_first = false;
			CaculationPoints();
		}
		var list = new List<Vertex>();
		var path = new List<LineMeshGenerator.Vertex>();
		for( int i = 0; i < _points.Count; i++ )
		{
			path.Add( new LineMeshGenerator.Vertex( _points[i].x, _points[i].y ) );
		}
		var mesh = LineMeshGenerator.Generate( path );

		for( int i = 0; i < mesh.Count; i++ )
		{
			var vert = new Vertex();
			vert.tint = Color.white;
			vert.position = new Vector3( mesh[i].x, mesh[i].y, Vertex.nearZ );
			list.Add( vert );
		}
		var indexList = new List<ushort>();
		for( int i = 0; i < _points.Count - 1; i++ )
		{
			var vertIndex = ( i * 4 );
			indexList.Add( (ushort)vertIndex );
			indexList.Add( (ushort)( vertIndex + 3 ) );
			indexList.Add( (ushort)( vertIndex + 1 ) );
			indexList.Add( (ushort)( vertIndex ) );
			indexList.Add( (ushort)( vertIndex + 2 ) );
			indexList.Add( (ushort)( vertIndex + 3 ) );
		}


		//for( int i = 0; i < _points.Count - 1; i++ )
		//{
		//	var dir = _points[i + 1] - _points[i];
		//	dir = dir.normalized;
		//	var vertical = NormalVector( dir );

		//	var vert1 = new Vertex();
		//	vert1.tint = Color.white;
		//	vert1.position = new Vector3( _points[i].x, _points[i].y, Vertex.nearZ );
		//	var vert2 = new Vertex();
		//	vert2.tint = Color.white;
		//	vert2.position = new Vector3( _points[i].x + vertical.x, _points[i].y + vertical.y, Vertex.nearZ );
		//	var vert3 = new Vertex();
		//	vert3.tint = Color.white;
		//	vert3.position = new Vector3( _points[i + 1].x, _points[i + 1].y, Vertex.nearZ );
		//	var vert4 = new Vertex();
		//	vert4.tint = Color.white;
		//	vert4.position = new Vector3( _points[i + 1].x + vertical.x, _points[i + 1].y + vertical.y, Vertex.nearZ );
		//	list.Add( vert1 );
		//	list.Add( vert2 );
		//	list.Add( vert3 );
		//	list.Add( vert4 );
		//}

		//var indexList = new List<ushort>();
		//for( int i = 0; i < _points.Count - 1; i++ )
		//{
		//	var vertIndex = ( i * 4 );
		//	indexList.Add( (ushort)vertIndex );
		//	indexList.Add( (ushort)( vertIndex + 3 ) );
		//	indexList.Add( (ushort)( vertIndex + 1 ) );
		//	indexList.Add( (ushort)( vertIndex ) );
		//	indexList.Add( (ushort)( vertIndex + 2 ) );
		//	indexList.Add( (ushort)( vertIndex + 3 ) );
		//}

		var data = context.Allocate( list.Count, indexList.Count );
		data.SetAllVertices( list.ToArray() );
		data.SetAllIndices( indexList.ToArray() );
	}
}