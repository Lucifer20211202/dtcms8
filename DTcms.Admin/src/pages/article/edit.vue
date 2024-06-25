<template>
	<div class="mainbody">
		<dt-location :data="[{title:'文章管理'},{title:'编辑内容'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="允许评论">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isComment"></el-switch>
						</el-form-item>
						<el-form-item label="选择类别">
							<dt-dropdown-check v-model="datas.form.categoryCheckedNodes"
								:data="datas.form.categoryList"
								:checked="datas.form.categoryCheckedKeys" />
						</el-form-item>
						<el-form-item v-if="datas.form.groupList.length>0" label="阅读权限">
							<el-checkbox-group v-model="datas.form.groupCheckedIds">
								<el-checkbox v-for="(item,i) in datas.form.groupList" :key="item.id" :label="item.id">{{item.title}}</el-checkbox>
							</el-checkbox-group>
						</el-form-item>
						<el-form-item label="显示状态">
							<el-radio-group v-model="datas.model.status">
								<el-radio-button label="0">正常</el-radio-button>
								<el-radio-button label="1">待审核</el-radio-button>
								<el-radio-button label="2">已删除</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item v-if="datas.form.labelList.length>0" label="标签属性">
							<el-checkbox-group v-model="datas.form.labelCheckedIds">
								<el-checkbox-button v-for="item in datas.form.labelList" :label="item.id" :key="item.id">{{item.title}}</el-checkbox-button>
							</el-checkbox-group>
						</el-form-item>
						<el-form-item prop="imgUrl" label="封面图片">
							<dt-upload-image v-model="datas.model.imgUrl" :size="datas.uploadConfig.imgSize" title="上传封面" />
						</el-form-item>
						<el-form-item prop="videoUrl" label="上传视频">
							<dt-upload-text v-model="datas.model.videoUrl" placeholder="支持URL或本地上传" exts="datas.uploadConfig.videoExtension" :size="datas.uploadConfig.videoSize" />
						</el-form-item>
						<el-form-item prop="title" label="文章标题">
						    <el-input v-model="datas.model.title" placeholder="必填，250字符内" show-word-limit maxlength="512"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字" :rules="datas.rules.number">
						    <el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
						</el-form-item>
						<el-form-item prop="click" label="浏览次数" :rules="datas.rules.number">
						    <el-input v-model="datas.model.click"></el-input>
						</el-form-item>
						<el-form-item prop="source" label="文章来源">
						    <el-input v-model="datas.model.source" placeholder="文章的来源地址"></el-input>
						</el-form-item>
						<el-form-item prop="author" label="文章作者">
						    <el-input v-model="datas.model.author" placeholder="文章作者"></el-input>
						</el-form-item>
						<el-form-item v-if="datas.form.channelModel.isAttach==1" label="附件上传">
							<dt-upload-attach v-model="datas.form.attachList" 
								:size="datas.uploadConfig.attachSize" 
								:exts="datas.uploadConfig.fileExtension" />
						</el-form-item>
						<el-form-item v-if="datas.form.channelModel.isAlbum==1" label="相册上传">
							<dt-upload-album v-model="datas.form.albumList"
								:twidth="datas.uploadConfig.thumbnailWidth"
								:theight="datas.uploadConfig.thumbnailHeight"
								:size="datas.uploadConfig.imgSize" />
						</el-form-item>
						<el-form-item label="发布时间">
							<el-date-picker v-model="datas.model.addTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" placeholder="发布时间" />
						</el-form-item>
						<el-form-item label="最后更新" v-if="datas.model.updateTime">
							{{datas.model.updateBy}} {{datas.model.updateTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="扩展选项" name="fields" v-if="datas.model.articleFields.length>0">
					<div class="tab-content">
						<el-form-item  v-for="(field,index) in datas.model.articleFields"
							:prop="`articleFields.${index}.fieldValue`"
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
								<el-date-picker v-model="field.fieldValue" value-format="YYYY-MM-DD hh:mm:ss" :placeholder="field.validTipMsg" type="datetime" />
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
				<el-tab-pane label="详细描述" name="details">
					<div class="tab-content">
						<el-form-item prop="callIndex" label="调用别名">
							<el-input v-model="datas.model.callIndex" maxlength="128" placeholder="非必填，单页可做为别名"></el-input>
						</el-form-item>
						<el-form-item prop="linkUrl" label="外部链接">
							<el-input type="text" v-model="datas.model.linkUrl"></el-input>
						</el-form-item>
						<el-form-item prop="zhaiyao" label="内容摘要">
							<el-input type="textarea" :row="3" v-model="datas.model.zhaiyao" show-word-limit maxlength="255"></el-input>
						</el-form-item>
						<el-form-item prop="content" label="内容介绍">
							<dt-editor v-model="datas.model.content" placeholder="请输入内容介绍"></dt-editor>
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="SEO选项" name="seo">
					<div class="tab-content">
						<el-form-item prop="seoTitle" label="SEO标题">
							<el-input type="text" v-model="datas.model.seoTitle"></el-input>
						</el-form-item>
						<el-form-item prop="seoKeyword" label="SEO关健字">
							<el-input type="text" v-model="datas.model.seoKeyword"></el-input>
						</el-form-item>
						<el-form-item prop="seoDescription" label="SEO描述">
							<el-input type="textarea" :row="3" v-model="datas.model.seoDescription" show-word-limit maxlength="512"></el-input>
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
	import DtFormBox from '../../components/layout/DtFormBox.vue'
	import DtEditor from '../../components/control/DtEditor.vue'
	import DtDropdownCheck from '../../components/control/DtDropdownCheck.vue'
	import DtUploadImage from '../../components/upload/DtUploadImage.vue'
	import DtUploadText from '../../components/upload/DtUploadText.vue'
	import DtUploadAlbum from '../../components/upload/DtUploadAlbum.vue'
	import DtUploadAttach from '../../components/upload/DtUploadAttach.vue'
	
	
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
		copyId: 0,
		uploadConfig: {}, //上传配置
		fieldRules: {}, //扩展字段验证规则
		form: {
			categoryList: [], //类别列表
			categoryCheckedKeys: [], //初始化选中健，修改赋值
			categoryCheckedNodes: [],//当前选中类别节点,保存取值
			albumList: [],//相册列表,保存取值
			attachList: [],//附件列表
			labelList: [],//标签列表
			labelCheckedIds: [],//标签选中值,保存取值
			groupList: [],//会员组列表
			groupCheckedIds: [],//阅读权限选中值,保存取值
			channelModel: {},//频道信息
		},
		model: {
			id: 0,
			channelId: props.channelId,
			callIndex: null,
			title: null,
			source: '本站',
			author: '管理员',
			linkUrl: null,
			imgUrl: null,
			videoUrl: null,
			seoTitle: null,
			seoKeyword: null,
			seoDescription: null,
			zhaiyao: null,
			content: null,
			sortId: 99,
			click: 100,
			status: 0,
			isComment: 1,
			addBy: null,
			addTime: null,
			categoryRelations: [],
			labelRelations: [],
			articleGroups: [],
			articleFields: [],
			articleAlbums: [],
			articleAttachs: [],
		},
		rules: {
			callIndex: [
				{ max: 128, message: '别名不可超出128字符', trigger: 'blur' }
			],
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
			linkUrl: [
				{ max: 512, message: '外部链接不可超出512字符', trigger: 'blur' }
			],
			imgUrl: [
				{ max: 512, message: '图片地址不可超出512字符', trigger: 'blur' }
			],
			videoUrl: [
				{ max: 512, message: '视频地址不可超出512字符', trigger: 'blur' }
			],
			seoTitle: [
				{ max: 512, message: 'SEO标题不可超出512字符', trigger: 'blur' }
			],
			seoKeyword: [
				{ max: 512, message: 'SEO关健字不可超出512字符', trigger: 'blur' }
			],
			seoDescription: [
				{ max: 512, message: 'SEO描述不可超出512字符', trigger: 'blur' }
			],
			zhaiyao: [
				{ max: 255, message: '内容摘要不可超出255字符', trigger: 'blur' }
			],
			content: [
				{ required: true, message: '内容不可为空', trigger: 'blur' },
			],
			categoryRelations: [
				{ type: 'array', required: true, message: '请选择栏目类别', trigger: 'blur' },
			],
			number: [
				{ pattern: /^\d+$/, message: '请输入正确的数字', trigger: 'blur' },
			],
		},
	})
	
	//初始化数据
	const initData = async() => {
		let articleId = props.id
		datas.copyId = proxy.$route.query.copyId //获取需要复制的ID
		//如果是复制
		if (datas.copyId) {
			articleId = parseInt(datas.copyId)
		}
		const loading = proxy.$loading() //开启Loading
		//上传配置
		await proxy.$api.request({
			url: `/client/setting/uploadconfig`,
			success(res) {
				datas.uploadConfig = res.data
			}
		})
		//获取可用标签
		await proxy.$api.request({
			url: `/admin/article/label/view/${datas.model.channelId}/100?status=0`,
			success(res) {
				datas.form.labelList = res.data
			}
		})
		//获取频道信息
		await proxy.$api.request({
			url: `/admin/channel/${props.channelId}`,
			success(res) {
				datas.form.channelModel = res.data
			}
		})
		//获取会员组列表
		await proxy.$api.request({
			url: '/admin/member/group?status=0',
			success(res) {
				datas.form.groupList = res.data
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
		if (articleId) {
			await proxy.$api.request({
				url: `/admin/article/${props.channelId}/${articleId}`,
				success(res) {
					datas.model = res.data
					//如果是复制，将ID设置为0
					if (datas.copyId) {
						datas.model.id = 0
					}
					//给标签选中赋值
					if (datas.model.labelRelations) {
						datas.form.labelCheckedIds = datas.model.labelRelations.map(item => item.labelId)
					}
					//给类别设置选中赋值
					if (datas.model.categoryRelations) {
						datas.form.categoryCheckedKeys = datas.model.categoryRelations.map(item => item.categoryId)
					}
					//赋值给会员组选中
					if (datas.model.articleGroups) {
						datas.form.groupCheckedIds = datas.model.articleGroups.map(item => item.groupId)
					}
				}
			})
		}
		
		//相册赋值
		if (datas.model.articleAlbums) {
			datas.model.articleAlbums.map((item) => {
				item.url = item.thumbPath
				datas.form.albumList.push(item)
			})
		}
		//附件赋值
		if (datas.model.articleAttachs) {
			datas.model.articleAttachs.map((item) => {
				datas.form.attachList.push(item)
			})
		}
		
		initFields() //初始化扩展字段
		loading.close() //关闭Loading
	}
	//初始化扩展字段
	const initFields = () => {
		//重置验证规则
		datas.fieldRules = {}
		//重置表单
		datas.form.channelModel.fields.forEach((val, i) => {
			//编辑的时候触发
			if (datas.model.id > 0 || props.copyId > 0) {
				//获得扩展字段值
				let obj = getFieldsValue(val.name)
				//构建Fields属性
				val.id = obj.id
				val.articleId = datas.model.id
				val.fieldId = val.id
				val.fieldName = val.name
				
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
				val.id = 0
				val.articleId = 0
				val.fieldId = val.id
				val.fieldName = val.name
			}
			//生成验证规则
			getRules(val)
		})
		datas.model.articleFields = datas.form.channelModel.fields
		
		//取得扩展字段的值
		function getFieldsValue(name) {
			let obj = { value: '', id: 0 }
			if (datas.model.articleFields.length > 0) {
				datas.model.articleFields.forEach((val, index) => {
					if (val.fieldName == name) {
						obj = { value: val.fieldValue, id: val.id }
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
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//检查及组合参数
				if (datas.form.categoryCheckedNodes.length === 0) {
					proxy.$message({ type: 'warning', message: '错误提示：请选择文章类别' })
					return false
				}
				//复制而不是赋值
				let model = JSON.parse(JSON.stringify(datas.model))
				
				//处理扩展字段
				model.articleFields.forEach((val, index) => {
					if (Array.isArray(val.fieldValue)) {
						val.fieldValue = val.fieldValue.join(',')
					}
					//如果是复制则将ID设置为0
					if (datas.copyId) {
						val.id = 0
					}
				})
				//分类取值
				let categoryRelations = []
				datas.form.categoryCheckedNodes.map((item) => {
					let modelt = {
						id: 0,
						articleId: model.id,
						categoryId: item.id
					}
					//修改时遍历Model已有值，防止ID丢失
					model.categoryRelations.map((v) => {
						if (modelt.categoryId === v.categoryId) {
							modelt.id = !datas.copyId ? v.id : 0 //复制id为0
						}
					})
					categoryRelations.push(modelt)
				})
				model.categoryRelations = categoryRelations
				//相册取值
				let albumList = []
				datas.form.albumList.map((item) => {
					albumList.push({
						id: !datas.copyId ? item.id : 0, //复制id为0
						articleId: datas.model.id,
						thumbPath: item.thumbPath,
						originalPath: item.originalPath,
						remark: item.remark
					})
				})
				model.articleAlbums = albumList
				//附件取值
				let attachList = []
				datas.form.attachList.map((item) => {
					attachList.push({
						id: !datas.copyId ? item.id : 0, //复制id为0
						fileName: item.fileName,
						articleId: datas.model.id,
						filePath: item.filePath,
						fileSize: item.fileSize,
						fileExt: item.fileExt,
						point: item.point,
						downCount: item.downCount
					})
				})
				model.articleAttachs = attachList
				//标签取值
				let labelRelations = []
				datas.form.labelCheckedIds.map((id) => {
					let modelt = {
						id: 0,
						articleId: datas.model.id,
						labelId: id
					}
					//修改时遍历Model已有值，防止ID丢失
					datas.model.labelRelations.map((v) => {
						if (modelt.labelId === v.labelId) {
							modelt.id = !datas.copyId ? v.id : 0 //复制id为0
						}
					})
					labelRelations.push(modelt)
				})
				model.labelRelations = labelRelations
				//会员组取值
				let articleGroups = []
				datas.form.groupCheckedIds.map((id) => {
					let modelt = {
						id: 0,
						articleId: datas.model.id,
						groupId: id
					}
					//修改时遍历Model已有值，防止ID丢失
					datas.model.articleGroups.map((v) => {
						if (modelt.groupId === v.groupId) {
							modelt.id = !datas.copyId ? v.id : 0 //复制id为0
						}
					})
					articleGroups.push(modelt)
				})
				model.articleGroups = articleGroups
				
				//Id大于0则修改，否则添加
				if (model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/article/${props.channelId}/${model.id}`,
						data: model,
						successMsg: '修改文章已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl(`/article/list/${props.channelId}`) //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: `/admin/article/${props.channelId}`,
						data: model,
						successMsg: '新增文章已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	
	//运行初始化
	initData()
</script>