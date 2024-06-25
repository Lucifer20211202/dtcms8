<template>
	<div class="mainbody">
		<dt-location :data="[{title:'投稿管理'},{title:'编辑投稿'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
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
							<dt-upload-image v-model="datas.model.imgUrl" :size="datas.uploadConfig.imgSize"></dt-upload-image>
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
							<dt-upload-album v-model="datas.form.albumList"></dt-upload-album>
						</el-form-item>
						<el-form-item v-show="datas.channelModel.isAttach==1" label="附件上传">
							<dt-upload-attach v-model="datas.form.attachList" :size="datas.uploadConfig.attachSize" :exts="datas.uploadConfig.fileExtension"></dt-upload-attach>
						</el-form-item>
						<el-form-item prop="content" label="内容介绍">
							<dt-editor v-model="datas.model.content" placeholder="请输入内容介绍"></dt-editor>
						</el-form-item>
						<el-form-item label="状态" v-if="datas.model.id>0">
							<el-radio-group v-model="datas.model.status" @change="handleChangeStatus">
								<el-radio-button label="0">待审</el-radio-button>
								<el-radio-button label="1">通过</el-radio-button>
								<el-radio-button label="2">驳回</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item v-if="datas.status!=1&&datas.model.status==1&&datas.model.id>0" label="选择类别">
							<dt-dropdown-check v-model="datas.form.categoryCheckedNodes"
								:data="datas.form.categoryList"
								:checked="datas.form.categoryCheckedKeys">
							</dt-dropdown-check>
						</el-form-item>
						<el-form-item label="创建时间" v-if="datas.model.id>0">
							{{datas.model.userName}} {{datas.model.addTime}}
						</el-form-item>
						<el-form-item label="最后更新" v-if="datas.model.updateTime">
							{{datas.model.updateBy}} {{datas.model.updateTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
				<template></template>
				<el-tab-pane label="扩展内容" name="fields" v-if="datas.model.fields.length>0">
					<div class="tab-content">
						<el-form-item  v-for="(field,index) in datas.model.fields" 
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
									<dt-editor :mini="field.editorType==1" v-if="field.editorType==0" v-model="field.fieldValue" :placeholder="field.validTipMsg" />
								</div>
							</template>
							<!--图片上传-->
							<template v-else-if="field.controlType=='images'">
								<dt-upload-image v-model="field.fieldValue" :title="field.title" :size="datas.uploadConfig.imgSize" />
							</template>
							<!--视频上传-->
							<template v-else-if="field.controlType=='video'">
								<dt-upload-text v-model="field.fieldValue" :placeholder="field.validTipMsg" :exts="datas.uploadConfig.videoExtension" :size="datas.uploadConfig.videoSize" />
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
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	import DtEditor from '../../../components/control/DtEditor.vue'
	import DtDropdownCheck from '../../../components/control/DtDropdownCheck.vue'
	import DtUploadText from '../../../components/upload/DtUploadText.vue'
	import DtUploadImage from '../../../components/upload/DtUploadImage.vue'
	import DtUploadAlbum from '../../../components/upload/DtUploadAlbum.vue'
	import DtUploadAttach from '../../../components/upload/DtUploadAttach.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		channelId: 0,
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		status: 0,
		uploadConfig: {}, //上传配置
		channelList: [], //频道列表
		channelModel: {},
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
		const loading = proxy.$loading()
		//上传配置
		await proxy.$api.request({
			url: `/client/setting/uploadconfig`,
			success(res) {
				datas.uploadConfig = res.data
			}
		})
		//频道信息
		await proxy.$api.request({
			url: `/admin/channel/${props.channelId}`,
			success(res) {
				datas.channelModel = res.data
			}
		})
		//获取频道列表
		await proxy.$api.request({
			url: `/admin/channel/view/0?siteId=${datas.channelModel.siteId}`,
			success(res) {
				datas.channelList = res.data
			}
		})
		//获取分类列表
		await proxy.$api.request({
			url: `/admin/article/category/${props.channelId}`,
			success(res) {
				datas.form.categoryList = res.data
			}
		})
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/article/contribute/${props.channelId}/${props.id}/view`,
				success(res) {
					//赋值给model
					datas.model = res.data
					//赋值扩展字段
					datas.status = datas.model.status
					//相册赋值
					if (datas.model.albums) {
						datas.model.albums.map((item) => {
							item.url = item.thumbPath
							item.id = 0
							datas.form.albumList.push(item)
						})
					}
					//附件赋值
					if (datas.model.attachs) {
						datas.model.attachs.map((item) => {
							item.id = 0
							datas.form.attachList.push(item)
						})
					}
				}
			})
		} else {
			//频道选项赋值
			handleChannelChange(props.channelId)
			datas.model.channelId = datas.channelModel.id
		}
		
		//获得频道扩展字段
		initFields()
		//关闭Loading
		loading.close()
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
				datas.form.albumList.map((item) => {
					albumList.push({
						id: 0,
						articleId: 0,
						thumbPath: item.thumbPath,
						originalPath: item.originalPath,
						remark: item.remark
					})
				});
				model.albums = albumList
				//附件取值
				let attachList = []
				datas.form.attachList.map((item) => {
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
				datas.form.categoryCheckedNodes.map((item) => {
				    categoryRelations.push(item.id)
				})
				model.categorys = categoryRelations
				
				//Id大于0则修改，否则添加
				if (model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/article/contribute/${model.channelId}/${model.id}`,
						data: model,
						successMsg: '投稿修改成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl(`/article/contribute/list/${props.channelId}`) //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: `/admin/article/contribute/${datas.channelId}`,
						data: model,
						successMsg: '投稿新增成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
							datas.form.albumList = []
							datas.form.attachList = []
							datas.model.albums = []
							datas.model.attachs = []
							datas.model.albumMeta = ''
							datas.model.attachMeta = ''
							datas.model.fieldMeta = ''
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	//频道变化
	const handleChannelChange = (val) => {
		getChannelModel(val)
	}
	//获得频道信息
	const getChannelModel = (id) => {
		proxy.$api.request({
			url: `/admin/channel/${id}`,
			success(res) {
				datas.channelModel = res.data
				datas.model.fields = datas.channelModel.fields
				//获得频道扩展字段
				initFields()
			}
		})
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
	//点击状态按钮
	const handleChangeStatus = (val) => {
		datas.form.categoryCheckedKeys = []
		datas.form.categoryCheckedNodes = []
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
	
	//运行初始化
	initData()
</script>