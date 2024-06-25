<template>
	<div class="mainbody">
		<dt-location :data="[{title:'文章管理'},{title:'编辑类别'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="是否启用">
							<el-switch :active-value="0" :inactive-value="1" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item prop="parentId" label="所属父类">
							<dt-dropdown-select v-model="datas.model.parentId" :data="datas.parentList" :disabled="datas.model.id" placeholder="请选择"></dt-dropdown-select>
						</el-form-item>
						<el-form-item prop="title" label="类别名称">
							<el-input v-model="datas.model.title" placeholder="必填，128字符内"></el-input>
						</el-form-item>
						<el-form-item prop="callIndex" label="调用别名">
							<el-input v-model="datas.model.callIndex" placeholder="任意字符，128个字符内"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input type="number" v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
						</el-form-item>
						<el-form-item label="创建时间" v-if="datas.model.id>0">
							{{datas.model.addBy}} {{datas.model.addTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="扩展选项" name="expand">
					<div class="tab-content">
						<el-form-item label="显示图片">
							<dt-upload-image v-model="datas.model.imgUrl"></dt-upload-image>
						</el-form-item>
						<el-form-item prop="linkUrl" label="外部链接">
							<el-input type="text" v-model="datas.model.linkUrl"></el-input>
						</el-form-item>
						<el-form-item label="类别介绍">
							<dt-editor v-model="datas.model.content" :mini="true" placeholder="请输入内容介绍"></dt-editor>
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
							<el-input type="textarea" v-model="datas.model.seoDescription"></el-input>
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
	import DtDropdownSelect from '../../../components/control/DtDropdownSelect.vue'
	import DtEditor from '../../../components/control/DtEditor.vue'
	import DtUploadImage from '../../../components/upload/DtUploadImage.vue'
	
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
		parentList: [], //父类别列表
		model: {
			id: 0,
			parentId: 0,
			channelId: props.channelId,
			title: null,
			callIndex: null,
			linkUrl: null,
			imgUrl: null,
			content: null,
			sortId: 99,
			status: 0,
			seoTitle: null,
			seoKeyword: null,
			seoDescription: null,
			addBy: null,
			addTime: null,
		},
		rules: {
			title: [
				{ required: true, message: '请输入分类名称', trigger: 'blur' },
			],
			callIndex: [
				{ max: 128, message: '调用别名不可超出128字符', trigger: 'blur' },
				{ pattern: /^[a-zA-Z_]{1,}$/, message: '只能字母和下划线', trigger: 'blur' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能为正整数', trigger: 'blur' },
			],
			linkUrl: [
				{ max: 512, message: '外部链接不可超出512字符', trigger: 'blur' }
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
		},
	})
	
	//初始化数据
	const initData = async() => {
		let parentId = proxy.$route.query.parentId //获取父ID
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/article/category/${props.channelId}/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		} else if (parentId) {
			datas.model.parentId = parseInt(parentId)
		}
		
		//加载父类列表
		await proxy.$api.request({
			url: `/admin/article/category/${props.channelId}`,
			success(res) {
				datas.parentList = res.data
			}
		})
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//如果是数组则取最后一个值
				if (Array.isArray(datas.model.parentId)) {
					datas.model.parentId = datas.model.parentId[datas.model.parentId.length - 1]
				}
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/article/category/${props.channelId}/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改类别已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl(`/article/category/list/${props.channelId}`) //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: `/admin/article/category/${props.channelId}`,
						data: datas.model,
						successMsg: '新增类别已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
							initData() //重新加载菜单
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