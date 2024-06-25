<template>
	<div class="mainbody">
		<dt-location :data="[{title:'应用管理'},{title:'广告管理'},{title:'编辑内容'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="是否启用">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item prop="advertId" label="所属广告位">
							<el-select v-model="datas.model.advertId" placeholder="请选择...">
								<el-option v-for="item in datas.advertList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="startTime" label="开始时间">
							<el-date-picker v-model="datas.model.startTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" placeholder="开始时间"></el-date-picker>
						</el-form-item>
						<el-form-item prop="endTime" label="结束时间">
							<el-date-picker v-model="datas.model.endTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" placeholder="结束时间"></el-date-picker>
						</el-form-item>
						<el-form-item prop="filePath" label="上传文件">
							<dt-upload-text v-model="datas.model.filePath"
								placeholder="请上传视频或图片文件"
								:exts="datas.uploadConfig.fileExtension"
								:size="datas.uploadConfig.attachSize"
								action="/upload">
							</dt-upload-text>
						</el-form-item>
						<el-form-item prop="title" label="内容标题">
							<el-input v-model="datas.model.title" placeholder="广告内容的标题，128字符内"></el-input>
						</el-form-item>
						<el-form-item prop="linkUrl" label="跳转链接">
							<el-input type="text" v-model="datas.model.linkUrl" placeholder="站外链接仅电脑端支持"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
						</el-form-item>
						<el-form-item prop="content" label="备注说明">
							<el-input type="textarea" :rows="5" v-model="datas.model.content" placeholder="支持HTML代码"></el-input>
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
	import DtFormBox from '../../../../components/layout/DtFormBox.vue'
	import DtUploadText from '../../../../components/upload/DtUploadText.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		uploadConfig: {},
		advertList: [],
		model: {
			id: 0,
			advertId: null,
			title: null,
			content: null,
			filePath: null,
			linkUrl: null,
			sortId: 99,
			status: 1,
			startTime: null,
			endTime: null,
			addBy: null,
			addTime: null,
		},
		rules: {
			advertId: [
				{ required: true, message: '请选择所属广告位', trigger: 'change' }
			],
			title: [
				{ required: true, message: '请输入内容标题', trigger: 'blur' }
			],
			startTime: [
				{ required: true, message: '请选择开始时间', trigger: 'change' }
			],
			endTime: [
				{ required: true, message: '请选择结束时间', trigger: 'change' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' }
			]
		},
	})
	
	//初始化数据
	const initData = async() => {
		//获取系统信息
		await proxy.$api.request({
			url: `/client/setting/uploadconfig`,
			success(res) {
				datas.uploadConfig = res.data
			}
		});
		//加载广告位列表
		await proxy.$api.request({
			url: '/admin/advert/view/0',
			success(res) {
				datas.advertList = res.data
			}
		});
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/advert/banner/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/advert/banner/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改广告内容已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/apply/advert/banner/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/advert/banner',
						data: datas.model,
						successMsg: '新增广告内容已成功',
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