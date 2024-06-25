<template>
	<div class="attach-box">
		<el-upload ref="uploadRef" class="list-btn"
			multiple
			list-type="picture"
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
			<el-button type="primary">{{placeholder}}</el-button>
			<template #tip>
				<div class="el-upload__tip" v-if="props.limit>0">只能上传 {{props.exts}} 文件，且不超过 {{props.size}}KB，最多可添加 {{props.limit}} 个文件。</div>
				<div class="el-upload__tip" v-else>只能上传 {{props.exts}} 文件，且不超过 {{props.size}}KB。</div>
			</template>
		</el-upload>
		<div class="list-wrap">
			<div class="list-box" v-for="(item,index) in datas.listData" :key="index"
				:draggable="true" @dragstart="dragFileStart(index)" @dragover="dragFileOver" @drop="dropFile">
				<div class="img-box">
					<span v-if="item.fileExt">{{item.fileExt.toUpperCase()}}</span>
					<el-icon v-else><Loading /></el-icon>
				</div>
				<div class="info-box">
					<h3>{{item.fileName}}</h3>
					<dl>
						<dt>
							<span v-if="item.fileSize">大小 {{handleFileSize(item.fileSize)}}</span>
							<span>积分 {{item.point}} 分</span>
							<span>下载 {{item.downCount}} 次</span>
						</dt>
						<dd>
							<span @click="handleDownload(item)"><el-icon><Download /></el-icon></span>
							<span @click="handleUpdate(item,index)"><el-icon><Edit /></el-icon></span>
						</dd>
					</dl>
				</div>
				<div class="close" @click="handleRemove(item)"><el-icon><Close /></el-icon></div>
				<el-progress v-if="item.progressFlag" :show-text="false" :percentage="item.progressPercent" />
			</div>
		</div>
		<!--附件设置-->
		<el-dialog title="积分设置" width="30%" v-model="datas.showUploadDialog" :fullscreen="datas.fullDialog" append-to-body>
			<div class="option-box">
				<div class="rows-box">
					<el-input type="number" v-model="datas.options.allPoint">
						<template #prepend>扣减</template>
						<template #append>分</template>
					</el-input>
				</div>
			</div>
			<template #footer>
				<div class="dialog-footer">
					<el-button type="primary" @click="handleSubmit">确认上传</el-button>
					<el-button @click="handCancel">取消</el-button>
				</div>
			</template>
		</el-dialog>
		<!--附件设置-->
		<el-dialog title="附件设置" width="30%" v-model="datas.showUpdateDialog" :fullscreen="datas.fullDialog" append-to-body>
			<div class="option-box">
				<h3>文件设置</h3>
				<div class="rows-box">
					<el-input v-model="datas.options.fileName" placeholder="请输入文件名">
						<template #prepend>名称</template>
					</el-input>
				</div>
				<div class="rows-box">
					<el-input type="number" v-model="datas.options.downCount">
						<template #prepend>下载</template>
						<template #append>次</template>
					</el-input>
				</div>
				<h3>积分设置</h3>
				<div class="rows-box">
					<el-input type="number" v-model="datas.options.point">
						<template #prepend>扣减</template>
						<template #append>分</template>
					</el-input>
				</div>
			</div>
			<template #footer>
				<div class="dialog-footer">
					<el-button type="primary" @click="handleUpdateConfirm">确认</el-button>
					<el-button @click="handleUpdateCancel">取消</el-button>
				</div>
			</template>
		</el-dialog>
	</div>
</template>

<script setup>
	import { ref,reactive,onMounted,getCurrentInstance,watchEffect } from "vue"
	import { ElMessage,ElMessageBox } from 'element-plus'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//通知父组件响应
	const emit = defineEmits(['update:modelValue'])
	//ref定义
	const uploadRef = ref(null)
	//接收props传值
	/*
	 * modelValue 绑定的值
	 * placeholder 显示文本
	 * limit 最大允许上传个数
	 * size 上传大小 单位KB 默认5MB
	 * exts 允许上传的类型 多个类型用逗号分开，例如 jpg,png,gif
	 * action 上传地址
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
		placeholder: {
			type: String,
			default: '点击上传',
		},
		limit: {
			type: Number,
			default: 0,
		},
		size: {
			type: Number,
			default: 5120,
		},
		exts: {
			type: String,
			default: 'rar,zip,7z,doc,docx,xls,xlsx,jpg,jpeg,png,gif,webp,bmp',
		}
	})
	//定义组件属性
	const datas = reactive({
		showUploadDialog: false,
		showUpdateDialog: false,
		fullDialog: false,
		listData: [],
		options: {
			index: -1,
			fileName: '',
			point: 0,
			downCount: 0,
			allPoint: 0,
		}
	})
	
	//上传前检查
	const handleBefore = (file) => {
		let isUploadExt = false
		let fileName = file.name
		let ext = '' //扩展名
		//获取最后一个.的位置
		let extIndex = fileName.lastIndexOf(".")
		if (extIndex === -1) {
			ElMessage.error('错误：不支持的上传类型')
			return false
		}
		ext = fileName.substring(extIndex + 1, fileName.length).toLowerCase()
		let extArr = props.exts.toLowerCase().split(',') //允许的扩展名数组
		//判断是否包含该扩展名
		extArr.forEach(item => {
			if (item === ext) {
				isUploadExt = true
			}
		})
		if (!isUploadExt) {
			ElMessage.error('错误：不支持的上传类型')
			return false
		}
		let fileSize = parseFloat(file.size / 1024);//转换成KB
		if (fileSize > props.size) {
			ElMessage.error(`错误：文件大小不能超过${props.size}KB`)
			return false
		}
		return true
	}
	//选取文件后
	const handleChange = (file) => {
		//打开设置Dialog
		datas.showUploadDialog = true
	}
	//提交上传
	const handleSubmit = () => {
		//关闭设置Dialog
		datas.showUploadDialog = false
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
			fileName: data.file.name,
			point: datas.options.allPoint,
			sortId: 99,
			downCount: 0,
			progressFlag: true,
			progressPercent: 0
		})
		datas.listData.push(itemObj)
		//开始上传文件
		proxy.$api.request({
			method: 'file',
			url: `${props.action}`,
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
				emit('update:modelValue', datas.listData)
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
		item.url = proxy.$api.loadFile(data[0].filePath)
		item.fileName = data[0].fileName
		item.filePath = data[0].filePath
		item.fileSize = data[0].fileSize
		item.fileExt = data[0].fileExt
		//通知更新
		emit('update:modelValue', datas.listData)
	}
	//超出文件限制
	const handleExceed = (files, fileList) => {
		//清空上传控制文件列表
		uploadRef.value.clearFiles()
		ElMessage.error("错误提示：超出上传最大限制数量")
	}
	//上传失败
	const handleError = (err, file, fileList) => {
		ElMessage.error('错误提示：上传失败，'+err.message)
	}
	//取消上传
	const handCancel = () => {
		//清空上传控制文件列表
		uploadRef.value.clearFiles()
		datas.showUploadDialog = false
	}
	//修改附件信息
	const handleUpdate = (item, i) => {
		//赋值临时变量
		datas.options.index = i
		datas.options.fileName = item.fileName
		datas.options.point = item.point
		datas.options.downCount = item.downCount
		//打开设置Dialog
		datas.showUpdateDialog = true
	}
	//确认修改附件信息
	const handleUpdateConfirm = () => {
		//查找要修改的对象
		if(datas.options.index === -1) return
		let itemObj = datas.listData[datas.options.index]
		//赋值要修改的项
		itemObj.fileName = datas.options.fileName
		itemObj.point = datas.options.point
		itemObj.downCount = datas.options.downCount
		//通知父组件更新
		emit('update:modelValue', datas.listData)
		//清空临时变量
		datas.options.index = -1
		//关闭设置Dialog
		datas.showUpdateDialog = false
	}
	//取消修改附件信息
	const handleUpdateCancel = () => {
		//清空临时变量
		datas.options.index = -1
		//关闭设置Dialog
		datas.showUpdateDialog = false
	}
	//下载文件
	const handleDownload = (item) => {
		window.location.href = item.url
	}
	//删除文件
	const handleRemove = (item) => {
		ElMessageBox.confirm('确认要删除文件吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		}).then(() => {
			//从列表中排除
			datas.listData = datas.listData.filter(val => val !== item)
			//通知父组件
			emit('update:modelValue', datas.listData)
			
		}).catch(() => {})
	}
	//转换大小
	const handleFileSize = (val) => {
		if (val < 1024.00)
			return val + "B"
		else if (val >= 1024.00 && val < 1048576)
			return parseInt(val / 1024.00) + "KB"
		else if (val >= 1048576 && val < 1073741824)
			return (val / 1024.00 / 1024.00).toFixed(2) + "MB"
		else if (val >= 1073741824)
			return (val / 1024.00 / 1024.00 / 1024.00).toFixed(2) + "GB"
	}
	//设置拖拽的数据（索引）
	const dragFileStart = (index) => {
		event.dataTransfer.setData('index', index)
	}
	//阻止默认的拖拽行为
	const dragFileOver = (event) => {
		event.preventDefault()
	}
	//获取拖拽的数据
	const dropFile = (event)=> {
		//文件的索引
		const oldIndex = event.dataTransfer.getData('index')
		const newIndex = event.target.getAttribute('key')
		//重新排序数组
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
					item.url = proxy.$api.loadFile(item.url)
				}
			})
		}
	})
	/*watch(() => props.modelValue, (newVal, oldVal) => {
		if (newVal.length > 0 && datas.listData.length == 0) {
			nextTick(() => {
				newVal.forEach(item => {
					if (item.url && !item.url.indexOf('http') == 0 && !item.url.indexOf('blob:http') == 0) {
						item.url = proxy.$api.loadFile(item.url)
					}
				})
				datas.listData = newVal
			})
		}
	})*/
</script>

<style lang="scss">
	.attach-box {
		.list-wrap {
			display: flex;
			flex-flow: row wrap;
			justify-content: space-between;
			margin-right: -20px;
			.list-box {
				display: block;
				position: relative;
				margin: 20px 20px 0 0;
				width: 230px;
				flex-grow: 1;
				box-sizing: border-box;
				border-radius: 4px;
				border: 1px solid #DCDFE6;
				cursor: move;
				overflow: hidden;
				.img-box {
					display: block;
					float: left;
					margin: 5px 10px 5px 5px;
					color: #fff;
					font-size: 24px;
					width: 60px;
					height: 60px;
					line-height: 60px;
					text-align: center;
					background: #67C23A;
				}
				.info-box {
					margin: 5px;
					h3 {
						margin-bottom: 10px;
						color: #606266;
						font-size: 14px;
						font-weight: 600;
						line-height: 1.5em;
						white-space: nowrap;
						text-overflow: ellipsis;
						overflow: hidden;
					}
					dl {
						display: flex;
						justify-content: space-between;
						margin-bottom: 5px;
						dt {
							color: #C0C4CC;
							font-size: 12px;
							line-height: 1em;
							white-space: nowrap;
							text-overflow: ellipsis;
							overflow: hidden;
							span {
								margin-right: 5px;
							}
						}
						dd {
							display: flex;
							justify-content: space-between;
							color: #67C23A;
							font-size: 20px;
							height: 20px;
							line-height: 20px;
							span {
								margin: 0 5px;
								cursor: pointer;
								&:hover {
									color: #409EFF;
								}
							}
						}
					}
				}
				.close {
					display: block;
					position: absolute;
					top: -20px;
					right: -20px;
					width: 40px;
					height: 40px;
					border-radius: 40px;
					color: #fff;
					font-size: 16px;
					background: #409EFF;
					cursor: pointer;
					overflow: hidden;
					&:hover {
						background: #E6A23C;
					}
					i {
						position: absolute;
						display: block;
						bottom: 4px;
						left: 3px;
					}
				}
			}
		}
	}
	.option-box {
		margin: -20px 0;
		h3 {
			margin-bottom: 15px;
			color: #1f2f3d;
			font-size: 16px;
			font-weight: 400;
		}
		.rows-box {
			margin: 15px 0;
		}
	}
</style>