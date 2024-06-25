<template>
	<div class="dt-upload-text">
		<div class="upload-box">
			<el-input :placeholder="props.placeholder" v-model="datas.filePath" @change="inputChange">
				<template v-if="datas.isImage" #prepend>
					<el-image :src="datas.imgSrc" fit="contain" :preview-src-list="new Array(datas.imgSrc)" />
				</template>
			</el-input>
			<el-upload ref="uploadRef"
				:disabled="datas.isLoading"
				:action="props.action"
				:show-file-list="false"
				:http-request="handleUpload"
				:before-upload="handleBefore"
				:on-error="handleError">
				<el-button type="primary" :loading="datas.isLoading">{{datas.buttonText}}</el-button>
			</el-upload>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,watch,nextTick,onMounted,getCurrentInstance,watchEffect } from "vue"
	import { ElMessage } from 'element-plus'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//通知父组件响应
	const emit = defineEmits(['update:modelValue'])
	//ref定义
	const uploadRef = ref(null)
	/*
	 * modelValue 绑定的值
	 * action 上传地址
	 * placeholder 显示文本
	 * size 上传大小 单位KB 默认1024KB
	 * water 是否加水印 1加水印0不加 默认0
	 * thumb 生成缩略图 1生成0不生成 默认0
	 * exts 允许上传的类型 多个类型用逗号分开，例如 jpg,png,gif
	 */
	const props = defineProps({
		modelValue: {
			type: String,
			default: null,
		},
		action: {
			type: String,
			default: '/upload',
		},
		placeholder: {
			type: String,
			default: null,
		},
		size: {
			type: Number,
			default: 5120,
		},
		water: {
			type: Number,
			default: 0,
		},
		thumb: {
			type: Number,
			default: 0,
		},
		exts: {
			type: String,
			default: 'jpg,jpeg,png,gif,bmp,webp',
		}
	})
	//定义组件属性
	const datas = reactive({
		filePath: null,
		imgSrc: null,
		isImage: false,
		isLoading: false,
		buttonText: '浏览...',
	})
	
	//上传前检查
	const handleBefore = (file) => {
		let isUploadExt = false
		let fileName = file.name
		//检查是否合法的扩展名
		if (fileName.lastIndexOf(".") === -1) {
			ElMessage.error('错误：不支持的上传类型')
			return false
		}
		let ext = fileName.substring(fileName.lastIndexOf(".") + 1, fileName.length).toLowerCase()
		let extArr = props.exts.toLowerCase().split(',') //允许的扩展名数组
		let extObj = extArr.find(val => val == ext)
		if (!extObj) {
			ElMessage.error('错误：不支持的上传类型')
			return false
		}
		//计算文件大小
		let fileSize = parseFloat(props.size / 1024) //转换成KB
		if (fileSize > props.size) {
			ElMessage.error(`错误：文件大小不能超过${props.size}KB`)
			return false
		}
		return true
	}
	//上传文件
	const handleUpload = (data) => {
		const formData = new FormData()
		formData.append("file", data.file)
		proxy.$api.request({
			method: 'file',
			url: `${props.action}?water=${props.water}&thumb=${props.thumb}`,
			data: formData,
			progress(event) {
				handleProcess(event, data.file)
			},
			beforeSend() {
				datas.isLoading = true
			},
			success(res) {
				handleSuccess(res.data, data.file)
			},
			complete() {
				datas.buttonText = '浏览...'
				datas.isLoading = false
				//清空上传控制文件列表
				uploadRef.value.clearFiles()
			}
		})
	}
	//上传成功
	const handleSuccess = (data, file) => {
		datas.filePath = data[0].filePath
		//通知更新
		emit('update:modelValue', datas.filePath)
	}
	//上传进度
	const handleProcess = (event, file) => {
		datas.buttonText = (event.loaded / event.total * 100).toFixed(0) + '%'
	}
	//上传失败
	const handleError = (err, file, fileList) => {
		ElMessage.error('错误：'+err.message)
	}
	//输入框变化
	const inputChange = () => {
		//通知更新
		emit('update:modelValue', datas.filePath)
	}
	//验证是否是图片
	const verifyImage = (src) => {
		if (!src) return false
		let imgExtArr = ['jpg', 'jpeg', 'png', 'gif', 'webp']
		let startIndex = src.lastIndexOf(".")
		if (startIndex === -1) {
			return false
		}
		let ext = src.substring(startIndex + 1, src.length).toLowerCase()
		let extObj = imgExtArr.find(val => val == ext)
		if (extObj) return true
		return false
	}
	//给预览图赋值
	const setImage = (src) => {
		//如果是图片则赋值给预览图
		datas.isImage = verifyImage(src)
		if (datas.isImage) {
			datas.imgSrc = proxy.$api.loadFile(src)
		}
	}
	
	//监视modelValue赋值的方法
	/*watch(() => props.modelValue, (newVal, oldVal) => {
		//if(!newVal) return
		datas.filePath = newVal
		setImage(newVal)
	})*/
	watchEffect(() => {
		datas.filePath = props.modelValue
		setImage(props.modelValue)
	})
	
</script>

<style lang="scss">
	.dt-upload-text {
		width: 100%;
		.upload-box {
			display: flex;
			justify-content: flex-start;
			align-items: center;
			.el-input-group__prepend{
				margin: 0;
				padding: 0;
				.el-image {
					padding: 1px 1px 1px 2px;
					height: var(--el-component-size);
					box-sizing: border-box;
				}
			}
			.el-input__wrapper {
				border-top-right-radius: 0px;
				border-bottom-right-radius: 0px;
			}
			.el-button {
				border-top-left-radius: 0px;
				border-bottom-left-radius: 0px;
			}
		}
	}
</style>