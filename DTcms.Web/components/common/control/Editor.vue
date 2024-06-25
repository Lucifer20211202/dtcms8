<template>
	<div class="editor-container">
		<Toolbar
			class="toolbar-box"
			:editor="editorRef"
			:defaultConfig="toolbarConfig"
			:mode="datas.mode"
		/>
		<Editor
			class="editor-box"
			v-model="datas.content"
			:defaultConfig="editorConfig"
			:mode="datas.mode"
			@onCreated="handleCreated"
			@onChange="handleChange"
		/>
	</div>
</template>

<script setup>
	import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
	
	//接收props传值
	const props = defineProps({
		mini: {
			type: Boolean,
			default: false,
		},
		modelValue: {
			type: String,
			default: ''
		},
		placeholder: {
			type: String,
			default: ''
		}
	})
	// 编辑器实例，必须用 shallowRef
	const editorRef = shallowRef()
	//通知父组件响应
	const emits = defineEmits(['update:modelValue'])
	//定义组件变量
	const datas = reactive({
		isload: false,
		content: '',
		mode: props.mini ? 'simple' : 'default',
	})
	
	//创建编辑器
	const handleCreated = (editor) => {
		editorRef.value = editor //记录 editor 实例，重要！
		//如果动态加载时有问题，加上下面这些代码
		if(props.modelValue){
			editorRef.value.setHtml(datas.content);
		}
	}
	//编辑器响应
	const handleChange = (editor) => {
		let html = editor.getHtml()
		//替换图片成相对路径
		let newContent = common.replaceEditor(html)
		//通知更新
		emits('update:modelValue', newContent)
	}
	//上传图片
	const handleUploadImage = (file, insertFn) => {
		const formData = new FormData()
		formData.append("file", file)
		//开始上传
		useHttp({
			method: 'file',
			url: '/upload',
			data: formData,
			loading: true,
			success(res) {
				let src = common.loadFile(res.data[0].filePath)
				let name = res.data[0].fileName
				insertFn(src, name, '')
			}
		})
	}
	//上传视频
	const handleUploadVideo = (file, insertFn) => {
		const formData = new FormData()
		formData.append("file", file)
		//开始上传
		useHttp({
			url: '/upload',
			method: 'file',
			data: formData,
			loading: true,
			success(res) {
				let src = common.loadFile(res.data[0].filePath)
				insertFn(src, '')
			}
		})
	}
	
	//编辑器工具栏配置
	const toolbarConfig = { }
	const editorConfig = {
		placeholder: props.placeholder,
		MENU_CONF: {
			//配置上传图片
			uploadImage: {
				customUpload: handleUploadImage
			},
			//配置上传视频
			uploadVideo: {
				customUpload: handleUploadVideo
			},
		}
	}
	
	//组件销毁时，也及时销毁编辑器
	onBeforeUnmount(() => {
		const editor = editorRef.value
		if (editor == null) return
		editor.destroy()
	})
	
	//监视modelValue赋值的方法
	watchEffect(() => {
		datas.content = common.replaceHtml(props.modelValue)
	})
</script>

<style lang="scss">
	.editor-container {
		border: 1px solid #DCDFE6;
		border-radius: 4px;
		z-index: 5;
		.toolbar-box {
			margin: 0;
			padding: 0;
			border-bottom: 1px solid #DCDFE6;
			border-radius: 4px 4px 0 0;
			overflow: inherit;
		}
		.editor-box {
			margin: 0;
			padding: 0;
			line-height: normal;
			min-height: 280px;
			border-radius: 4px;
			overflow: hidden;
		}
	}
</style>