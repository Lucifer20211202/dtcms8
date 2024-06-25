<template>
	<common-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
		<el-tab-pane label="编辑稿件" name="info">
			<el-form-item prop="channelId" label="所属频道">
				<el-select v-model="datas.model.channelId" :disabled="datas.model.id>0" placeholder="请选择..." @change="handleChannelChange">
					<el-option v-for="item in datas.channelList"
						:key="item.id"
						:label="item.title"
						:value="item.id">
					</el-option>
				</el-select>
			</el-form-item>
			<el-form-item prop="imgUrl" label="封面图片">
				<common-upload-image v-model="datas.model.imgUrl" :size="uploadConfig.imgSize" />
			</el-form-item>
			<el-form-item prop="title" label="文章标题">
				<el-input v-model="datas.model.title" placeholder="必填，512字符内"></el-input>
			</el-form-item>
			<el-form-item prop="source" label="文章来源">
				<el-input v-model="datas.model.source" placeholder="必填，128字符内"></el-input>
			</el-form-item>
			<el-form-item prop="author" label="文章作者">
				<el-input v-model="datas.model.author" placeholder="必填，128字符内"></el-input>
			</el-form-item>
			<el-form-item v-show="datas.channelModel.isAlbum==1" label="相册上传">
				<common-upload-album v-model="datas.form.albumList" :twidth="uploadConfig.thumbnailWidth" :theight="uploadConfig.thumbnailHeight" />
			</el-form-item>
			<el-form-item v-show="datas.channelModel.isAttach==1" label="附件上传">
				<common-upload-attach v-model="datas.form.attachList" :size="uploadConfig.attachSize" :exts="uploadConfig.fileExtension" />
			</el-form-item>
			<el-form-item prop="content" label="内容介绍">
				<common-control-editor v-model="datas.model.content" placeholder="请输入内容介绍" />
			</el-form-item>
			
			<el-form-item  v-for="(field,index) in datas.model.fields"
				:key="index"
				:prop="`fields.${index}.fieldValue`"
				:rules="datas.fieldRules[field.name]"
				:label="field.title">
				<!--单行文本框-->
				<template v-if="field.controlType=='input'">
					<el-input v-if="field.isPassword==0" v-model="field.fieldValue" :placeholder="field.validTipMsg" />
					<el-input v-else v-model="field.fieldValue" :placeholder="field.validTipMsg" show-password />
				</template>
				<!--多行文本框-->
				<template v-else-if="field.controlType=='textarea'">
					<el-input type="textarea" v-model="field.fieldValue" :placeholder="field.validTipMsg" />
				</template>
				<!--编辑器-->
				<template v-else-if="field.controlType=='editor'">
					<div style="min-height:230px;">
						<common-control-editor :mini="field.editorType==1" v-if="field.editorType==0" v-model="field.fieldValue" :placeholder="field.validTipMsg" />
					</div>
				</template>
				<!--图片上传-->
				<template v-else-if="field.controlType=='images'">
					<common-upload-image v-model="field.fieldValue" :title="field.title" :size="uploadConfig.imgSize" />
				</template>
				<!--视频上传-->
				<template v-else-if="field.controlType=='video'">
					<common-upload-text v-model="field.fieldValue" :placeholder="field.validTipMsg" :exts="uploadConfig.videoExtension" :size="uploadConfig.videoSize" />
				</template>
				<!--日期时间-->
				<template v-else-if="field.controlType=='datetime'">
					<el-date-picker v-model="field.fieldValue" value-format="YYYY-MM-DD HH:mm:ss" :placeholder="field.validTipMsg" type="datetime" />
				</template>
				<!--多项单选-->
				<template v-else-if="field.controlType=='radio'">
					<el-radio-group v-model="field.fieldValue">
						<el-radio v-for="(item,index) in field.options" :label="item[1]" :key="item[1]">{{item[0]}}</el-radio>
					</el-radio-group>
				</template>
				<!--多项多选-->
				<template v-else-if="field.controlType=='checkbox'">
					<el-checkbox-group v-model="field.fieldValue">
						<el-checkbox v-for="(item,index) in field.options" :label="item[1]" :key="item[1]">{{item[0]}}</el-checkbox>
					</el-checkbox-group>
				</template>
			</el-form-item>
			
			<el-form-item>
				<el-button type="primary" :loading="datas.btnLoading" @click="submitForm">确认提交</el-button>
				<el-button @click="common.back(-1)">返回上一页</el-button>
			</el-form-item>
		</el-tab-pane>
	</common-form-box>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `编辑稿件 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	//获取路由信息
	const route = useRoute()
	//获取上传配置信息
	const uploadConfig = await useSite('upload')
	//获取当前会员信息
	const userInfo = await useUser('info')
	//Ref对象
	const editFormRef = ref(null)
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		btnLoading: false,
		status: 0,
		channelList: [], //频道列表
		channelModel: {}, //当前频道
		fieldRules: {},
		form: {
			categoryList: [], //类别列表
			categoryCheckedKeys: [], //初始化选中健，修改赋值
			categoryCheckedNodes: [],//当前选中类别节点,保存取值
			albumList: [],//相册列表,保存取值
			attachList: [],//附件列表,保存取值
		},
		model: {
			id: 0,
			channelId: null,
			title: null,
			source: null,
			author: null,
			imgUrl: null,
			fieldMeta: null,
			albumMeta: null,
			attachMeta: null,
			content: null,
			userId: 0,
			userName: null,
			addTime: null,
			updateBy: null,
			updateTime: null,
			status: 0,
			fields: [],
			categorys: [],
			albums: [],
			attachs: [],
		},
		rules: {
			title: [
				{ required: true, message: '标题不可为空', trigger: 'blur' },
				{ max: 512, message: '标题不可超出512字符', trigger: 'blur' }
			],
			source: [
				{ max: 128, message: '来源不可超出128字符', trigger: 'blur' }
			],
			author: [
				{ max: 128, message: '作者不可超出128字符', trigger: 'blur' }
			],
			imgUrl: [
				{ max: 512, message: '图片地址不可超出512字符', trigger: 'blur' }
			],
			categorys: [
				{ type: 'array', required: true, message: '请选择栏目类别', trigger: 'blur' },
			],
		},
	})
	
	//初始化数据
	const initData = async() => {
		//启用loading
		datas.loading = true
		//获取频道列表
		await useHttp({
			url: `/client/channel/view/0`,
			success(res) {
				datas.channelList = []
				res.data.forEach((item, index) => {
					if (item.isContribute == 1 && item.status==0) {
						datas.channelList.push(item)
					}
				})
			}
		}).catch((err) => {})
		//修改时赋值
		if (route.params.id) {
			await useHttp({
				url: `/account/article/contribute/${route.params.id}`,
				success(res) {
					//赋值给model
					datas.model = res.data
					//赋值扩展字段
					datas.status = datas.model.status
					//相册赋值
					if (datas.model.albums) {
						datas.model.albums.forEach((item) => {
							item.url = item.thumbPath
							item.id = 0
							datas.form.albumList.push(item)
						})
					}
					//附件赋值
					if (datas.model.attachs) {
						datas.model.attachs.forEach((item) => {
							item.id = 0
							datas.form.attachList.push(item)
						})
					}
				}
			}).catch((err) => {})
			//频道选项赋值
			handleChannelChange(datas.model.channelId)
		} else if(route.query.channelId) {
			//频道选项赋值
			handleChannelChange(route.query.channelId)
			datas.model.channelId = datas.channelModel.id
		}
		//关闭Loading
		datas.loading = false
	}
	//获得频道信息
	const handleChannelChange = async(val) => {
		await useHttp({
			url: `/client/channel/${val}`,
			success(res) {
				datas.channelModel = res.data
				datas.model.fields = datas.channelModel.fields
				//获得频道扩展字段
				initFields()
		
			}
		}).catch((err) => {})
	}
	//初始化扩展字段
	const initFields = () => {
		//重置验证规则
		datas.fieldRules = {}
		//重置表单
		datas.channelModel.fields.forEach((val, i) => {
			//编辑的时候触发
			if (datas.model.id > 0) {
				//获得扩展字段值
				let obj = getFieldsValue(val.name)
				//构建Fields属性
				val.fieldName = val.name
				val.fieldId = obj.id
				//如果值存在，将值替换默认值
				if (obj.value) {
					//如果是数组
					if (Array.isArray(val.fieldValue)) {
						val.fieldValue = obj.value.split(',')
					} else {
						val.fieldValue = obj.value
					}
	
				} else {
					if (Array.isArray(val.fieldValue)) {
						val.fieldValue = []
					} else {
						val.fieldValue = ''
					}
	
				}
			} else {
				//构建Fields属性
				val.fieldName = val.name
				val.fieldId = val.id
			}
			//生成验证规则
			getRules(val)
		})
		datas.model.fields = datas.channelModel.fields
		
		//取得扩展字段的值
		function getFieldsValue(name) {
			let obj = { value: '', id: 0 }
			let fields = JSON.parse(datas.model.fieldMeta)
			if (fields.length > 0) {
				fields.forEach((val, index) => {
					if (val.fieldName == name) {
						obj = { value: val.fieldValue, id: val.fieldId }
					}
				})
			}
			return obj
		}
		//生成验证规则
		function getRules(obj) {
			let item = []
			//blur验证的控件
			if (obj.controlType == 'input' ||
				obj.controlType == 'textarea' ||
				obj.controlType == 'editor' ||
				obj.controlType == 'images' ||
				obj.controlType == 'video' ||
				obj.controlType == 'datetime'
			) {
				//必填项
				if (obj.isRequired == 1) {
					let eleRule = { required: true, message: obj.title + '不能为空', trigger: 'blur' }
					item.push(eleRule)
				}
				//检查正则是否为null
				if (!(!obj.validPattern && typeof (obj.validPattern) != 'undefined' && obj.validPattern != 0)) {
					//不为空，将字符串转成正则表达式
					let eleRule = { pattern: eval(obj.validPattern), message: obj.validErrorMsg, trigger: 'blur' }
					item.push(eleRule)
				}
	
			} else if (obj.controlType == 'radio' ||
				obj.controlType == 'checkbox') {
				//change验证的控件
				//必填项
				if (obj.isRequired == 1) {
					let eleRule = { required: true, message: obj.title + '不能为空', trigger: 'change' }
					item.push(eleRule)
				}
				//检查正则是否为null
				if (!(!obj.validPattern && typeof (obj.validPattern) != 'undefined' && obj.validPattern != 0)) {
					//不为空，将字符串转成正则表达式
					let eleRule = { pattern: eval(obj.validPattern), message: obj.validErrorMsg, trigger: 'change' }
					item.push(eleRule)
				}
			}
			let name = obj.name
			datas.fieldRules[name] = item
		}
	}
	//格式化相册数据
	const formatAlbum = () => {
		let albumsArr = []
		if (datas.model.albums) {
			let albums = datas.model.albums
			for (let i = 0; i < albums.length; i++) {
				if (albums[i].status === 'success') {
					albumsArr.push(albums[i])
				}
			}
			datas.model.albums = albumsArr
		}
	}
	//格式化附件数据
	const formatAttach = () => {
		let attachArr = []
		if (datas.model.attachs) {
			let attachs = datas.model.attachs
			for (let i = 0; i < attachs.length; i++) {
				if (attachs[i].status === 'success') {
					attachArr.push(attachs[i]);
				}
			}
			datas.model.attachs = attachArr
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//这里是复制不是赋值
				let model = JSON.parse(JSON.stringify(datas.model))
				//处理扩展字段
				model.fields.forEach((val, index) => {
					if (Array.isArray(val.fieldValue)) {
						val.fieldValue = val.fieldValue.join(',')
					}
				})
				//格式化相册数据去除没有上传成功的数据
				formatAlbum()
				//格式化附件数据去除没有上传成功的数据
				formatAttach()
				//相册取值
				let albumList = []
				datas.form.albumList.forEach((item) => {
					albumList.push({
						id: 0,
						articleId: 0,
						thumbPath: item.thumbPath,
						originalPath: item.originalPath,
						remark: item.remark
					})
				})
				model.albums = albumList
				//附件取值
				let attachList = []
				datas.form.attachList.forEach((item) => {
					attachList.push({
						id: 0,
						fileName: item.fileName,
						articleId: 0,
						filePath: item.filePath,
						fileSize: item.fileSize,
						fileExt: item.fileExt,
						downCount: item.downCount,
					})
				})
				model.attachs = attachList
				//分类取值
				let categoryRelations = []
				datas.form.categoryCheckedNodes.forEach((item) => {
				    categoryRelations.push(item.id)
				})
				model.categorys = categoryRelations
				
				//Id大于0则修改，否则添加
				if (model.id > 0) {
					useHttp({
						method: 'put',
						url: `/account/article/contribute/${model.id}`,
						data: model,
						successMsg: '编辑投稿成功',
						beforeSend() {
							datas.btnLoading = true
						},
						success(res) {
							navigateTo('/account/contribute') //跳转加列表页
						},
						complete() {
							datas.btnLoading = false
						}
					})
				} else {
					useHttp({
						method: 'post',
						url: `/account/article/contribute`,
						data: model,
						successMsg: '投稿提交成功',
						beforeSend() {
							datas.btnLoading = true
						},
						success(res) {
							navigateTo('/account/contribute') //跳转加列表页
						},
						complete() {
							datas.btnLoading = false
						}
					})
				}
			}
		})
	}
	
	//页面完成后执行
	onMounted(() => {
		initData()
	})
</script>