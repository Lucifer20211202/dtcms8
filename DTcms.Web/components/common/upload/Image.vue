<template>
	<div class="dt-upload-image">
		<div class="image-box" @click="datas.showDialog=true" :style="'width:'+props.boxwh+'px;height:'+props.boxwh+'px;'">
			<img v-if="datas.imgSrc" :src="datas.imgSrc" />
			<el-icon v-else-if="!datas.uploadFlag"><ElIconPlus /></el-icon>
			<el-progress v-if="datas.uploadFlag" type="circle" :percentage="datas.uploadPercent" :width="props.boxwh-22" />
		</div>
		<el-dialog v-model="datas.showDialog" :title="props.title" :fullscreen="datas.fullDialog" append-to-body>
			<div class="cropper-wrap">
				<div class="canvas-box">
					<div v-if="datas.imgSrc&&!options.imgSource" class="img-box">
						<img :src="datas.imgSrc" />
					</div>
					<vue-cropper v-else ref="cropperRef"
						:info="true"
						:full="true"
						:infoTrue="true"
						:img="options.imgSource"
						:outputSize="options.outputSize"
						:output-type="options.outputType"
						:fixed="options.fixed"
						:fixedNumber="options.fixedNumber"
						:fillColor="options.fillColor">
					</vue-cropper>
				</div>
				<div class="btns-box">
					<div class="list-box">
						<el-button-group>
							<el-button :icon="ElIconCrop" :disabled="!options.imgSource" @click="cropStart"></el-button>
							<el-button :icon="ElIconRank" :disabled="!options.imgSource" @click="cropStop"></el-button>
							<el-button :icon="ElIconRefreshLeft" :disabled="!options.imgSource" @click="cropRotateLeft"></el-button>
							<el-button :icon="ElIconRefreshRight" :disabled="!options.imgSource" @click="cropRotateRight"></el-button>
							<el-button :icon="ElIconPlus" :disabled="!options.imgSource" @click="cropChangeScale(1)"></el-button>
							<el-button :icon="ElIconMinus" :disabled="!options.imgSource" @click="cropChangeScale(-1)"></el-button>
						</el-button-group>
						<el-select v-model="options.fixedSelect"
							:disabled="!options.imgSource"
							placeholder="截图比例"
							@change="cropFixedNumber"
							clearable>
							<el-option v-for="(item,index) in options.fixedItems"
								:key="index"
								:label="item.label"
								:value="item.value">
							</el-option>
						</el-select>
					</div>
					<div class="list-box">
						<el-upload ref="uploadRef" accept="image/*"
							:disabled="datas.uploadFlag"
							:action="props.action"
							:show-file-list="false"
							:auto-upload="false"
							:http-request="handleSourceUpload"
							:before-upload="handleBefore"
							:on-change="handleChange"
							:on-error="handleError">
							<el-button :icon="ElIconFolderOpened">浏览..</el-button>
						</el-upload>
						<el-input placeholder="请输入图片网址" v-model="options.imgUri" clearable>
							<template #append>
								<el-button :icon="ElIconUploadFilled" @click="cropLoadRemote">载入</el-button>
							</template>
						</el-input>
					</div>
				</div>
			</div>
			<template #footer>
				<div class="dialog-footer">
					<el-button type="primary" @click="handleCropUpload" :disabled="datas.uploadFlag">上传截图</el-button>
					<el-button type="success" @click="submitSourceUpload" :disabled="datas.uploadFlag">上传原图</el-button>
					<el-button @click="datas.showDialog=false">取消</el-button>
				</div>
			</template>
		</el-dialog>
	</div>
</template>

<script setup>
	import { VueCropper } from 'vue-cropper'
	import 'vue-cropper/dist/index.css'
	
	//通知父组件响应
	const emits = defineEmits(['update:modelValue'])
	//ref定义
	const cropperRef = ref(null)
	const uploadRef = ref(null)
	//接收props传值
	const props = defineProps({
		modelValue: {
			type: String,
			default: null,
		},
		action: {
			type: String,
			default: '/upload',
		},
		size: {
			type: Number,
			default: 5120,
		},
		title: {
			type: String,
			default: '上传图片',
		},
		boxwh: {
			type: Number,
			default: 98,
		}
	})
	//定义组件属性
	const datas = reactive({
		showDialog: false,
		fullDialog: false,
		imgSrc: '',
		uploadFlag: false,
		uploadPercent: 0
	})
	//定义裁剪属性
	const options = reactive({
		remoteType: 0, // 远程代理
		imgUri: '', // 远程文件网址
		imgSource: '', // 画布图片地址
		fixed: false, // 是否开启截图框宽高固定比例
		fixedSelect: '', //选择框选中的值，分开绑定
		fixedNumber: [1,1], // 截图框的宽高比例, 开启fixed生效
		fillColor: '#ffffff', //导出时背景颜色填充
		outputType: 'webp', // jpeg, png, webp
		outputSize: 0.8, // 裁剪生成图片的质量0.1-1
		fixedItems: [
			{label: '1：1', value: '1,1'},
			{label: '4：3', value: '4,3'},
			{label: '3：4', value: '3,4'},
			{label: '16：9', value: '16,9'},
			{label: '9：16', value: '9,16'}
		]
	})
	
	//选取后检查
	const handleBefore = (file) => {
		const isImage = /^image\/\w+$/i.test(file.type)
		if (!isImage) {
			ElMessage.error('错误：只能上传图片文件')
			return false
		}
		let fileSize = parseFloat(file.size / 1024) //字节转为KB
		if (fileSize > props.size) {
			ElMessage.error(`错误：图片大小不能超过${props.size}KB`)
			return false
		}
		return true
	}
	//选取文件后
	const handleChange = (file) => {
		options.imgSource = window.URL.createObjectURL(file.raw)
		options.imgUri = '' //清空远程图片URL
	}
	//上传裁剪图片
	const handleCropUpload = () => {
		//检查是否有上传文件
		if(!options.imgSource) {
			ElMessage.error('错误提示：没有找到上传文件');
			return;
		}
		//关闭Dialog窗体
		datas.showDialog = false
		//开始上传
		cropperRef.value.getCropBlob((data) => {
			const formData = new FormData()
			formData.append("file", data)
			useHttp({
				method: 'file',
				url: `${props.action}`,
				data: formData,
				progress(event) {
					handleProcess(event);
				},
				beforeSend() {
					datas.uploadFlag = true
					datas.uploadPercent = 0
				},
				success(res) {
					options.imgSource = '' // 请空文件
					handleSuccess(res.data)
				},
				complete() {
					datas.uploadFlag = false
					datas.uploadPercent = 0
					//清空上传控制文件列表
					uploadRef.value.clearFiles()
				}
			})
		})
	}
	//上传原图
	const handleSourceUpload = (data) => {
		const formData = new FormData()
		formData.append("file", data.file)
		useHttp({
			method: 'file',
			url: `${props.action}`,
			data: formData,
			progress(event) {
				handleProcess(event)
			},
			beforeSend() {
				datas.uploadFlag = true
				datas.uploadPercent = 0
			},
			success(res) {
				options.imgSource = '' // 请空文件
				handleSuccess(res.data)
			},
			complete() {
				datas.uploadFlag = false
				datas.uploadPercent = 0
				//清空上传控制文件列表
				uploadRef.value.clearFiles()
			}
		})
	}
	//上传成功
	const handleSuccess = (data) => {
		//拼接得到图片url
		const imageUrl = data[0].filePath
		datas.imgSrc = common.loadFile(imageUrl)
		//触发事件，父组件会修改绑定的value值
		emits('update:modelValue', imageUrl)
	}
	//上传进度
	const handleProcess = (event) => {
		datas.uploadPercent = Math.abs((event.loaded / event.total * 100).toFixed(0))
	}
	//上传出错时
	const handleError = (err) => {
		ElMessage.error('错误：' + err)
	}
	//启用裁剪功能
	const cropStart = () => {
		if(!options.imgSource) return
		cropperRef.value.startCrop()
	}
	//取消裁剪
	const cropStop = () => {
		if(!options.imgSource) return
		cropperRef.value.stopCrop()
	}
	//图片左旋转
	const cropRotateLeft = () => {
		if(!options.imgSource) return
		cropperRef.value.rotateLeft()
	}
	//图片右旋转
	const cropRotateRight = () => {
		if(!options.imgSource) return
		cropperRef.value.rotateRight()
	}
	//图片缩放
	const cropChangeScale = (num) => {
		if(!options.imgSource) return
		cropperRef.value.changeScale(num)
	}
	//图片比例
	const cropFixedNumber = () => {
		//清除截图
		cropperRef.value.clearCrop()
		//检查是否没有选中值
		if(!!options.fixedSelect) {
			// 开启比例
			if(!options.fixed) options.fixed = true
			// 设置比例值
			options.fixedNumber = options.fixedSelect.split(',')
		} else {
			//关闭比例
			options.fixed = false
		}
	}
	//载入远程图片
	const cropLoadRemote = () => {
		if(!options.imgUri) return
		let uriSource = options.imgUri.trim()
		//不使用代理直接赋值
		options.imgSource = uriSource
	}
	//提交原图上传
	const submitSourceUpload = () => {
		//检查远程图片URL，如果非空则直接赋值地址
		if(!!options.imgUri) {
			//检查是否有上传文件
			if(!options.imgUri.startsWith("http")) {
				ElMessage.error('错误：图片的URL格式有误');
				return
			}
			datas.showDialog = false //关闭Dialog
			//触发事件，父组件会修改绑定的value值
			emits('update:modelValue', options.imgUri)
			return
		}
		//检查是否有上传文件
		if(!options.imgSource) {
			ElMessage.error('错误：没有找到上传文件');
			return
		}
		//关闭Dialog窗体
		datas.showDialog = false
		//否则使用上传控制
		uploadRef.value.submit()
	}
	
	//页面挂载完成
	onMounted(() => {
		//如果页面宽度小于600则全屏Dialog
		if(window.innerWidth < 600) datas.fullDialog = true
	})
	
	//监视modelValue赋值的方法
	watchEffect(() => {
		datas.imgSrc = common.loadFile(props.modelValue)
	})
</script>

<style lang="scss">
	.image-box {
		position: relative;
		display: flex;
		justify-content: center;
		align-items: center;
		border: 1px dashed #d9d9d9;
		border-radius: 6px;
		box-sizing: border-box;
		cursor: pointer;
		&>i {
			color: #888;
			font-size: 24px;
		}
		&>img {
			margin: auto;
			max-width: 100%;
			max-height: 100%
		}
		.el-progress{
			position: absolute;
			top: 0;
			left: 0;
			margin: 10px;
		}
	}
	.cropper-wrap {
		position: relative;
		.canvas-box {
			width: 100%;
			height: 35vh;
			.img-box {
				display: flex;
				justify-content: center;
				align-items: center;
				width: 100%;
				height: 100%;
				box-sizing: border-box;
				border: 1px solid #E4E7ED;
				border-radius: 4px;
				&>img {
					margin: auto;
					max-width: 100%;
					max-height: 100%
				}
			}
		}

		.btns-box {
			.list-box {
				display: flex;
				justify-content: space-between;
				margin: 10px -5px 0 -5px;
				&> div {
					margin: 0 5px;
					&:first-child {
						flex-shrink: 0;
					}
					&:last-child {
						flex-grow: 1;
					}
				}
			}
		}
	}
</style>