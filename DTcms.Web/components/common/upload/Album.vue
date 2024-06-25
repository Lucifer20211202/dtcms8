<template>
	<div class="upload-album-box">
		<div class="list-box" v-for="(item,index) in datas.listData" :key="index"
			:draggable="true" @dragstart="dragImageStart(index)" @dragover="dragImageOver" @drop="dropImage">
			<div class="img-box">
				<img :src="item.url" />
			</div>
			<span class="text-box" v-if="item.remark">{{item.remark}}</span>
			<div class="tools-box">
				<span class="icon" @click="handleRemove(item)">
					<el-icon><ElIconDelete /></el-icon>
				</span>
				<span class="icon" @click="handlePreview(item)">
					<el-icon><ElIconZoomIn /></el-icon>
				</span>
				<span class="icon" @click="handleRemark(item)">
					<el-icon><ElIconEdit /></el-icon>
				</span>
			</div>
			<el-progress v-if="item.progressFlag" type="circle" :width="96" :percentage="item.progressPercent" />
		</div>
		<el-upload ref="uploadRef" class="list-bin"
			multiple
			list-type="picture-card"
			accept="image/*"
			:action="props.action"
			:http-request="handleUpload"
			:before-upload="handleBefore"
			:on-change="handleChange"
			:on-error="handleError"
			:on-exceed="handleExceed" 
			:limit="props.limit"
			:file-list="datas.listData"
			:show-file-list="false" 
			:auto-upload="false">
			<el-icon><ElIconPlus /></el-icon>
		</el-upload>
		<!--图片预览-->
		<el-dialog title="图片预览" v-model="datas.showImgDialog" :fullscreen="datas.fullDialog" append-to-body>
			<div class="preview-box">
				<img :src="datas.showImgUrl" />
			</div>
		</el-dialog>
		<!--图片设置-->
		<el-dialog title="相册设置" width="30%" v-model="datas.showSizeDialog" :fullscreen="datas.fullDialog" append-to-body>
			<div class="option-box">
				<template v-if="props.twidth>0|| props.theight>0">
					<h3>缩略图尺寸</h3>
					<div class="rows-box">
						<el-input type="number" v-model="datas.options.width">
							<template #prepend>宽</template>
							<template #append>px</template>
						</el-input>
					</div>
					<div class="rows-box">
						<el-input type="number" v-model="datas.options.theight">
							<template #prepend>高</template>
							<template #append>px</template>
						</el-input>
					</div>
				</template>
				<h3>图片描述</h3>
				<div class="rows-box">
					<el-input type="textarea" :rows="3" placeholder="请输入图片描述" v-model="datas.options.remark"></el-input>
				</div>
			</div>
			<template #footer>
				<div class="dialog-footer">
					<el-button type="primary" @click="handleSubmit">确认上传</el-button>
					<el-button @click="handCancel">取消</el-button>
				</div>
			</template>
		</el-dialog>
	</div>
</template>

<script setup>
	//通知父组件响应
	const emits = defineEmits(['update:modelValue'])
	//ref定义
	const uploadRef = ref(null)
	//接收props传值
	/*
	 * modelValue 绑定的数组
	 * action 上传地址
	 * limit 最大允许上传个数，0不限制
	 * size 上传大小 单位KB 默认5120KB
	 * water 是否加水印 1加0不加 默认1
	 * thumb 生成缩略图 1生成0不生成 默认1
	 * exts 允许上传的类型 多个类型用逗号分开，例如 jpg,png,gif
	 * twidth 生成缩略图宽度
	 * theight 生成缩略图高度
	 */
	const props = defineProps({
		modelValue: {
			type: Array,
			default: [],
		},
		action: {
			type: String,
			default: '/upload',
		},
		size: {
			type: Number,
			default: 5120,
		},
		limit: {
			type: Number,
			default: 0
		},
		water: {
			type: Number,
			default: 1
		},
		thumb: {
			type: Number,
			default: 1
		},
		twidth: {
			type: Number,
			default: 0
		},
		theight: {
			type: Number,
			default: 0
		}
	})
	//定义组件属性
	const datas = reactive({
		listData: [],
		showImgDialog: false,
		showImgUrl: null,
		showSizeDialog: false,
		fullDialog: false,
		options: {
			width: 0,
			height: 0,
			remark: ''
		}
	})
	
	//上传前检查
	const handleBefore = (file) => {
		const isImg = /^image\/\w+$/i.test(file.type)
		if (!isImg) {
			ElMessage.error('只能上传 JPG、PNG、GIF 格式')
			return false
		}
		let fileSize = parseFloat(file.size / 1024) //字节转为KB
		if (fileSize > props.size) {
			ElMessage.error(`图片大小不能超过${props.size}KB`)
			return false
		}
		return true
	}
	//选取文件后
	const handleChange = (file) => {
		//打开设置Dialog
		datas.showSizeDialog = true
	}
	//提交上传
	const handleSubmit = () => {
		//关闭设置Dialog
		datas.showSizeDialog = false
		uploadRef.value.submit()
	}
	//上传文件
	const handleUpload = (data) => {
		const formData = new FormData()
		formData.append("file", data.file)
		
		//添加到列表
		let itemObj = reactive({
			id: 0,
			url: window.URL.createObjectURL(data.file),
			thumbPath: '',
			originalPath: '',
			remark: datas.options.remark,
			sortId: 99,
			progressFlag: true,
			progressPercent: 0
		})
		datas.listData.push(itemObj)
		//发送上传请求
		let sendUrl = `${props.action}?water=${props.water}&thumb=${props.thumb}`
		if(datas.options.width && datas.options.height) {
			sendUrl += `&twidth=${datas.options.width}&theight=${datas.options.height}`
		}
		useHttp({
			method: 'file',
			url: sendUrl,
			data: formData,
			progress(event) {
				handleProcess(event, itemObj)
			},
			success(res) {
				handleSuccess(res.data, itemObj)
			},
			error() {
				//从列表中排除
				datas.listData = datas.listData.filter(item => item !== itemObj)
				//通知更新
				emits('update:modelValue', datas.listData)
			},
			complete() {
				itemObj.progressFlag = false
				itemObj.progressPercent = 0
				//清空上传控制文件列表
				uploadRef.value.clearFiles()
			}
		})
	}
	//上传进度
	const handleProcess = (event, item) => {
		item.progressPercent = Math.abs((event.loaded / event.total * 100).toFixed(0))
	}
	//上传成功
	const handleSuccess = (data, item) => {
		item.url = common.loadFile(data[0].thumbPath[0])
		item.thumbPath = data[0].thumbPath[0]
		item.originalPath = data[0].filePath
		//通知更新
		emits('update:modelValue', datas.listData)
	}
	//超出限制
	const handleExceed = (files, fileList) => {
		//清空上传控制文件列表
		uploadRef.value.clearFiles()
		ElMessage.error("错误：超出上传最大限制数量")
	}
	//上传失败
	const handleError = (err, file, fileList) => {
		ElMessage.error('错误：' + err)
	}
	//取消上传
	const handCancel = () => {
		//清空上传控制文件列表
		uploadRef.value.clearFiles()
		datas.showSizeDialog = false
	}
	//删除文件
	const handleRemove = (item) => {
		ElMessageBox.confirm('确认要删除该图片吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		}).then(() => {
			//从列表中排除
			datas.listData = datas.listData.filter(val => val !== item)
			//通知更新
			emits('update:modelValue', datas.listData)
		}).catch(() => {})
	}
	//修改备注
	const handleRemark = (item) => {
		ElMessageBox.prompt("请输入备注：", "图片描述", {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			inputValue: item.remark,
			closeOnClickModal: false,
		}).then(({ value }) => {
			item.remark = value
			emits('update:modelValue', datas.listData)
		}).catch(() => {})
	}
	//查看图片
	const handlePreview = (item) => {
		datas.showImgUrl = common.loadFile(item.originalPath)
		datas.showImgDialog = true
	}
	//设置拖拽的数据（图片的索引）
	const dragImageStart = (index) => {
		event.dataTransfer.setData('index', index)
	}
	//阻止默认的拖拽行为
	const dragImageOver = (event) => {
		event.preventDefault()
	}
	//获取拖拽的数据
	const dropImage = (event)=> {
		//图片的索引
		const oldIndex = event.dataTransfer.getData('index')
		const newIndex = event.target.getAttribute('key')
		//重新排序图片数组
		const movedImage = datas.listData.splice(oldIndex, 1)[0]
		datas.listData.splice(newIndex, 0, movedImage)
		datas.listData.forEach((item,i) => item.sortId = i)
	}
	
	//页面挂载完成
	onMounted(() => {
		//如果页面宽度小于600则全屏Dialog
		if(window.innerWidth < 600) datas.fullDialog = true
	})
	
	//监视modelValue赋值的方法
	watchEffect(() => {
		if (props.modelValue.length > 0 && datas.listData.length == 0) {
			datas.listData = props.modelValue
			datas.listData.forEach(item => {
				if (item.url && !item.url.indexOf('http') == 0 && !item.url.indexOf('blob:http') == 0) {
					item.url = common.loadFile(item.url)
				}
			})
		}
		if(props.twidth > 0 && props.theight > 0) {
			datas.options.width = props.twidth
			datas.options.height = props.theight
		}
	})
</script>

<style lang="scss">
	.upload-album-box {
		display: flex;
		flex-flow: row wrap;
		justify-content: flex-start;
		margin-right: -20px;
		margin-bottom: -20px;
		&.small {
			margin-right: -10px;
			.el-upload--picture-card {
				width: 106px;
				height: 106px;
			}
			.list-bin {
				margin: 0 10px 10px 0;
			}
			.list-box {
				margin: 0 10px 10px 0;
				width: 106px;
				height: 106px;
			}
		}
		.el-upload--picture-card {
			display: flex;
			align-items: center;
			justify-content: center;
			width: 98px;
			height: 98px;
			border-radius: 4px;
			&>i {
				font-size: 24px;
			}
		}
		.list-bin {
			margin: 0 20px 20px 0;
		}
		.list-box {
			display: block;
			position: relative;
			margin: 0 20px 20px 0;
			width: 98px;
			height: 98px;
			box-sizing: border-box;
			border-radius: 4px;
			border: 1px solid #DCDFE6;
			cursor: move;
			overflow: hidden;
			&:hover {
				.tools-box {
					opacity: 1;
					transition: opacity .3s;
				}
			}
			.el-progress {
				position: absolute;
				top: 0;
				left: 0;
				background-color: rgba(0, 0, 0, 0.3);
				.el-progress__text {
					color: #409EFF;
				}
			}
			.img-box {
				display: flex;
				justify-content: center;
				align-items: center;
				width: 100%;
				height: 100%;
				img {
					max-width: 100%;
					max-height: 100%;
				}
			}
			.text-box {
				display: block;
				position: absolute;
				left: 0;
				bottom: 0;
				box-sizing: border-box;
				padding: 0 10px;
				width: 100%;
				color: #606266;
				font-size: 12px;
				text-shadow: 1px 1px 1px #fff;
				height: 28px;
				line-height: 28px;
				background-color: rgba(255, 255, 255, 0.4);
				white-space: nowrap;
				text-overflow: ellipsis;
				overflow: hidden;
			}
			.tools-box {
				display: flex;
				justify-content: center;
				align-items: center;
				position: absolute;
				width: 100%;
				height: 100%;
				color: #fff;
				font-size: 20px;
				left: 0;
				top: 0;
				opacity: 0;
				transition: opacity .3s;
				background-color: rgba(0, 0, 0, 0.5);
				.icon {
					display: block;
					margin: 5px;
					width: 20px;
					height: 20px;
					line-height: 20px;
					cursor: pointer;
				}
			}
		}
	}
	.preview-box {
		margin: -20px 0;
		img {
			width: 100%;
		}
	}
	.option-box {
		margin: 0 0 -10px;
		h3 {
			margin-bottom: 15px;
			color: #1f2f3d;
			font-size: 14px;
			font-weight: 400;
		}
		.rows-box {
			margin: 15px 0;
		}
	}
</style>