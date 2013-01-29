

namespace flash.display3D {

	using MonoMac.OpenGL;
	using MonoMac.AppKit;

	using System;
	using System.IO;
	using flash.events;
	using flash.display;
	using flash.utils;
	using flash.geom;
	using flash.display3D;
	using flash.display3D.textures;
	using _root;
	
	public class Context3D : EventDispatcher {
	
		//
		// Properties
		//
	
		public string driverInfo { get { return "MonoGL"; } }

		public bool enableErrorChecking { get; set; }

		//
		// Methods
		//


		
		public Context3D(Stage3D stage3D)
		{
			mStage3D = stage3D;
			setupShaders();
		}
		
		private void setupShaders ()
		{
			// we have hardcoded a few common shaders here
			mUntexturedProgram = createProgram();
			mUntexturedProgram.uploadFromGLSLFiles("untextured.vert", "untextured.frag");
			
			mTexturedProgram = createProgram();
			mTexturedProgram.uploadFromGLSLFiles("textured.vert", "textured.frag");
		}
		
		
		public void clear(double red = 0.0, double green = 0.0, double blue = 0.0, double alpha = 1.0, 
		                  double depth = 1.0, uint stencil = 0, uint mask = 0xffffffff) {
			
			GL.ClearColor (NSColor.FromDeviceRgba((float)red,(float)green,(float)blue,(float)alpha));
			GL.ClearDepth(depth);
			GL.ClearStencil((int)stencil);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
		}
		
		public void configureBackBuffer(int width, int height, int antiAlias, 
			bool enableDepthAndStencil = true, bool wantsBestResolution = false) {
		}
	
		public CubeTexture createCubeTexture(int size, string format, bool optimizeForRenderToTexture, int streamingLevels = 0) {
			throw new NotImplementedException();
		}

 	 	public IndexBuffer3D createIndexBuffer(int numIndices) {
 	 		return new IndexBuffer3D(this, numIndices);
 	 	}
 	 	
		public Program3D createProgram() {
			return new Program3D(this);
		}
 	 	
		public Texture createTexture(int width, int height, string format, 
			bool optimizeForRenderToTexture, int streamingLevels = 0) {
			return new Texture(this, width, height, format, optimizeForRenderToTexture, streamingLevels);
		}

 	 	public VertexBuffer3D createVertexBuffer(int numVertices, int data32PerVertex) {
 	 		return new VertexBuffer3D(this, numVertices, data32PerVertex);
 	 	}
 	 	
		public void dispose() {
			throw new NotImplementedException();
		}
 	 	
		public void drawToBitmapData(BitmapData destination) {
		 	throw new NotImplementedException();
		}
 	 	
		public void drawTriangles(IndexBuffer3D indexBuffer, int firstIndex = 0, int numTriangles = -1) {
			int count = (numTriangles == -1) ? indexBuffer.numIndices : (numTriangles * 3);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer.id);
			GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedInt, firstIndex );
		}
 	 	
		public void present() {
			GL.Flush();
		}

 	 	
		public void setBlendFactors(string sourceFactor, string destinationFactor) {
		}
 	 	
		public void setColorMask(bool red, bool green, bool blue, bool alpha) {
		}
 	 	
		public void setCulling (string triangleFaceToCull)
		{
			switch (triangleFaceToCull) {
			case "none":
				GL.Disable(EnableCap.CullFace);
				break;
			default:
				throw new NotImplementedException();
				// GL.CullFace(CullFaceMode.
			}
		}
 	 	
		public void setDepthTest(bool depthMask, string passCompareMode) {
		}
 	 	
		public void setProgram(Program3D program) {
			// $$TODO
		}
 	 	
		public void setProgramConstantsFromByteArray(string programType, int firstRegister, 
			int numRegisters, ByteArray data, uint byteArrayOffset) {
		}
 	 	
		public void setProgramConstantsFromMatrix(string programType, int firstRegister, Matrix3D matrix, 
			bool transposedMatrix = false) {
			
			// $$ hack
			// treat any matrix submitted as the projection matrix

			GL.MatrixMode (MatrixMode.Projection);
			// GL.LoadMatrix( matrix.);
			
			var array = new float[16];
			for (int i=0; i < 16; i++)
			{
				array[i] = (float) matrix.mData[i];
			}
			
			GL.LoadMatrix(array);
			
			
			GL.MatrixMode (MatrixMode.Modelview);
			GL.LoadIdentity();
		}
 	 	
		public void setProgramConstantsFromVector(string programType, int firstRegister, Vector<double> data, int numRegisters = -1) {
			//throw new NotImplementedException();
		}
 	 	
 	 	public void setRenderToBackBuffer() {
			// throw new NotImplementedException();
		}
 	 	
		public void setRenderToTexture(TextureBase texture, bool enableDepthAndStencil = false, int antiAlias = 0, 
		                               int surfaceSelector = 0) {
			throw new NotImplementedException();
		}


		public void setScissorRectangle(Rectangle rectangle) {
			throw new NotImplementedException();
		}

		public void setStencilActions(string triangleFace = "frontAndBack", string compareMode = "always", string actionOnBothPass = "keep", 
			string actionOnDepthFail = "keep", string actionOnDepthPassStencilFail = "keep") {
			throw new NotImplementedException();
		}
 	 	
		public void setStencilReferenceValue(uint referenceValue, uint readMask = 255, uint writeMask = 255) {
			throw new NotImplementedException();
		}

		public void setTextureAt (int sampler, TextureBase texture)
		{
			if (texture != null) {
				GL.ActiveTexture(TextureUnit.Texture0 + sampler);
				GL.BindTexture (TextureTarget.Texture2D, texture.textureId);
				GL.UseProgram(mTexturedProgram.programId);

			} else {
				GL.ActiveTexture(TextureUnit.Texture0 + sampler);
				GL.BindTexture (TextureTarget.Texture2D, 0);
				GL.UseProgram(mUntexturedProgram.programId);
			}
		}

		public void setVertexBufferAt (int index, VertexBuffer3D buffer, int bufferOffset = 0, string format = "float4")
		{
			if (buffer == null) {
				GL.DisableVertexAttribArray (index);
				GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
				return;
			}
		
			// enable vertex attribute array
			GL.EnableVertexAttribArray (index);
			GL.BindBuffer (BufferTarget.ArrayBuffer, buffer.id);

			int byteOffset = (bufferOffset * 4); // buffer offset is in 32-bit words

			// set attribute pointer within vertex buffer
			switch (format) {
			case "float4":
				GL.VertexAttribPointer(index, 4, VertexAttribPointerType.Float, false, buffer.stride, byteOffset);
				break;
			case "float3":
				GL.VertexAttribPointer(index, 3, VertexAttribPointerType.Float, false, buffer.stride, byteOffset);
				break;
			case "float2":
				GL.VertexAttribPointer(index, 2, VertexAttribPointerType.Float, false, buffer.stride, byteOffset);
				break;
			case "float1":
				GL.VertexAttribPointer(index, 1, VertexAttribPointerType.Float, false, buffer.stride, byteOffset);
				break;
			default:
				throw new NotImplementedException();
			}

		}

		private readonly Stage3D mStage3D;
	
		private Program3D mUntexturedProgram;
		private Program3D mTexturedProgram;
	}

}
